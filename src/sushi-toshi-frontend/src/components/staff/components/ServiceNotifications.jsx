// // File: sushi-toshi-frontend/components/staff/components/ServiceNotifications.jsx
// import { useState, useEffect, useCallback } from "react";
// import { Bell, X, Wifi, WifiOff } from "lucide-react";
// import { axiosInstance, createApiUrl } from "../../../config/api";

// const ServiceNotifications = () => {
//   const [notifications, setNotifications] = useState([]);
//   const [activeSessions, setActiveSessions] = useState([]);
//   const [sockets, setSockets] = useState({});
//   const [error, setError] = useState(null);
//   const [connectionStatus, setConnectionStatus] = useState({});

//   // Debug logging wrapper
//   const logDebug = (message, data = null) => {
//     const logMessage = `[ServiceNotifications] ${message}`;
//     if (data) {
//       console.log(logMessage, data);
//     } else {
//       console.log(logMessage);
//     }
//   };

//   // Fetch active sessions
//   const fetchActiveSessions = useCallback(async () => {
//     try {
//       // const token = localStorage.getItem("access_token");
//       // if (!token) {
//       //   throw new Error("No authentication token found");
//       // }

//       logDebug("Fetching active sessions...");

//       // const response = await fetch("http://localhost:8000/dashboard/sessions", {
//       //   headers: {
//       //     Authorization: `Bearer ${token}`,
//       //   },
//       // });

//       // if (!response.ok) {
//       //   throw new Error("Failed to fetch sessions");
//       // }

//       const response = await axiosInstance.get(
//         createApiUrl("/dashboard/sessions")
//       );
//       const sessions = response.data;
//       const activeSessionIds = sessions
//         .filter((session) => !session.ended_at)
//         .map((session) => session.session_id);

//       logDebug("Active sessions found:", activeSessionIds);
//       setActiveSessions(activeSessionIds);
//     } catch (err) {
//       logDebug("Error fetching sessions:", err);
//       setError("Failed to connect to notification service: " + err.message);
//     }
//   }, []);

//   // Connect WebSocket for a session
//   const connectWebSocket = useCallback(
//     (sessionId) => {
//       if (sockets[sessionId]) {
//         logDebug(`Socket already exists for session ${sessionId}`);
//         return;
//       }

//       logDebug(`Attempting to connect WebSocket for session ${sessionId}`);

//       try {
//         const token = localStorage.getItem("access_token");
//         if (!token) {
//           throw new Error("No authentication token found");
//         }

//         // Create WebSocket URL with explicit protocol
//         const wsProtocol =
//           window.location.protocol === "https:" ? "wss:" : "ws:";
//         const wsUrl = `${wsProtocol}//localhost:8000/service-requests/ws/${sessionId}?token=${token}`;

//         logDebug(`Connecting to WebSocket URL: ${wsUrl}`);
//         const ws = new WebSocket(wsUrl);

//         // Set a connection timeout
//         const connectionTimeout = setTimeout(() => {
//           if (ws.readyState !== WebSocket.OPEN) {
//             logDebug("WebSocket connection timeout");
//             ws.close();
//           }
//         }, 5000);

//         ws.onopen = () => {
//           clearTimeout(connectionTimeout);
//           logDebug(`WebSocket connected for session ${sessionId}`);
//           setConnectionStatus((prev) => ({
//             ...prev,
//             [sessionId]: "connected",
//           }));
//         };

//         ws.onmessage = (event) => {
//           logDebug(`Received message for session ${sessionId}:`, event.data);
//           try {
//             const data = JSON.parse(event.data);
//             if (data.type === "service_requests_update") {
//               logDebug(
//                 `Processing ${data.requests.length} requests for session ${sessionId}`
//               );

//               const newNotifications = data.requests.map((req) => ({
//                 id: req.request_id,
//                 sessionId: sessionId,
//                 title: `Service Request - Table ${req.table_number}`,
//                 message: req.notes || "No details provided",
//                 status: req.status,
//                 timestamp: new Date(req.created_at),
//                 isNew: !notifications.some((n) => n.id === req.request_id),
//               }));

//               setNotifications((prev) => {
//                 const existingIds = prev.map((n) => n.id);
//                 const uniqueNew = newNotifications.filter(
//                   (n) => !existingIds.includes(n.id)
//                 );
//                 logDebug(`Adding ${uniqueNew.length} new notifications`);
//                 return [...prev, ...uniqueNew];
//               });
//             }
//           } catch (err) {
//             logDebug("Error processing WebSocket message:", err);
//           }
//         };

//         ws.onclose = (event) => {
//           logDebug(
//             `WebSocket disconnected for session ${sessionId}. Code: ${event.code}, Reason: ${event.reason}`
//           );
//           setConnectionStatus((prev) => ({
//             ...prev,
//             [sessionId]: "disconnected",
//           }));
//           setSockets((prev) => {
//             const newSockets = { ...prev };
//             delete newSockets[sessionId];
//             return newSockets;
//           });
//           // Attempt to reconnect after 5 seconds
//           setTimeout(() => connectWebSocket(sessionId), 5000);
//         };

//         ws.onerror = (error) => {
//           logDebug(`WebSocket error for session ${sessionId}:`, error);
//           setConnectionStatus((prev) => ({
//             ...prev,
//             [sessionId]: "error",
//           }));
//           ws.close();
//         };

//         setSockets((prev) => ({
//           ...prev,
//           [sessionId]: ws,
//         }));
//       } catch (err) {
//         logDebug("Error creating WebSocket:", err);
//         setError(`Failed to connect to session ${sessionId}: ${err.message}`);
//         setConnectionStatus((prev) => ({
//           ...prev,
//           [sessionId]: "error",
//         }));
//       }
//     },
//     [notifications, sockets]
//   );

//   // Initial setup and cleanup
//   useEffect(() => {
//     logDebug("Initial fetch of active sessions");
//     fetchActiveSessions();
//     const intervalId = setInterval(fetchActiveSessions, 30000);

//     return () => {
//       clearInterval(intervalId);
//       Object.values(sockets).forEach((socket) => {
//         if (socket.readyState === WebSocket.OPEN) {
//           socket.close();
//         }
//       });
//     };
//   }, [fetchActiveSessions]);

//   // Handle session changes
//   useEffect(() => {
//     logDebug("Processing active sessions:", activeSessions);

//     // Connect to new sessions
//     activeSessions.forEach((sessionId) => {
//       if (!sockets[sessionId]) {
//         connectWebSocket(sessionId);
//       }
//     });

//     // Cleanup inactive sessions
//     Object.entries(sockets).forEach(([sessionId, socket]) => {
//       if (!activeSessions.includes(Number(sessionId))) {
//         logDebug(`Closing socket for inactive session ${sessionId}`);
//         if (socket.readyState === WebSocket.OPEN) {
//           socket.close();
//         }
//         setSockets((prev) => {
//           const newSockets = { ...prev };
//           delete newSockets[sessionId];
//           return newSockets;
//         });
//       }
//     });
//   }, [activeSessions, sockets, connectWebSocket]);

//   const handleDismiss = (notificationId) => {
//     logDebug(`Dismissing notification ${notificationId}`);
//     setNotifications((prev) => prev.filter((n) => n.id !== notificationId));
//   };

//   const handleClaimRequest = async (notification) => {
//     try {
//       logDebug(`Claiming request ${notification.id}`);
//       const token = localStorage.getItem("access_token");
//       const response = await fetch(
//         `http://localhost:8000/service-requests/${notification.id}/claim`,
//         {
//           method: "POST",
//           headers: {
//             Authorization: `Bearer ${token}`,
//           },
//         }
//       );

//       if (!response.ok) {
//         throw new Error("Failed to claim request");
//       }
//       logDebug(`Successfully claimed request ${notification.id}`);
//       handleDismiss(notification.id);
//     } catch (err) {
//       logDebug("Error claiming request:", err);
//       setError("Failed to claim request: " + err.message);
//     }
//   };

//   return (
//     <div className="fixed bottom-4 right-4 z-50">
//       {/* Connection Status Indicators */}
//       <div className="mb-2 flex flex-wrap gap-2 justify-end">
//         {Object.entries(connectionStatus).map(([sessionId, status]) => (
//           <div
//             key={sessionId}
//             className={`
//               flex items-center gap-1 px-2 py-1 rounded text-xs
//               ${
//                 status === "connected"
//                   ? "bg-green-100 text-green-800"
//                   : status === "disconnected"
//                   ? "bg-red-100 text-red-800"
//                   : "bg-yellow-100 text-yellow-800"
//               }
//             `}
//           >
//             {status === "connected" ? (
//               <Wifi className="w-3 h-3" />
//             ) : (
//               <WifiOff className="w-3 h-3" />
//             )}
//             Session #{sessionId}
//           </div>
//         ))}
//       </div>

//       {/* Error Display */}
//       {error && (
//         <div className="mb-2 p-2 bg-red-100 text-red-800 rounded-lg shadow text-sm">
//           {error}
//           <button
//             onClick={() => setError(null)}
//             className="ml-2 text-red-600 hover:text-red-800"
//           >
//             <X className="w-4 h-4" />
//           </button>
//         </div>
//       )}

//       {/* Notifications Stack */}
//       <div className="space-y-2">
//         {notifications.map((notification) => (
//           <div
//             key={notification.id}
//             className={`
//               transform transition-all duration-300 ease-in-out
//               p-4 rounded-lg shadow-lg max-w-sm bg-white
//               ${
//                 notification.status === "PENDING"
//                   ? "border-l-4 border-yellow-500"
//                   : "border-l-4 border-blue-500"
//               }
//               ${notification.isNew ? "animate-bounce" : ""}
//             `}
//           >
//             <div className="flex items-start justify-between">
//               <div className="flex items-start">
//                 <div className="flex-shrink-0">
//                   <Bell className="h-6 w-6 text-blue-600" />
//                 </div>
//                 <div className="ml-3">
//                   <p className="text-sm font-medium text-gray-900">
//                     Session #{notification.sessionId}: {notification.title}
//                   </p>
//                   <p className="mt-1 text-sm text-gray-500">
//                     {notification.message}
//                   </p>
//                   <p className="mt-1 text-xs text-gray-400">
//                     {notification.timestamp.toLocaleTimeString()}
//                   </p>
//                   {notification.status === "PENDING" && (
//                     <button
//                       onClick={() => handleClaimRequest(notification)}
//                       className="mt-2 text-sm text-blue-600 hover:text-blue-800"
//                     >
//                       Claim Request
//                     </button>
//                   )}
//                 </div>
//               </div>
//               <button
//                 onClick={() => handleDismiss(notification.id)}
//                 className="ml-4 text-gray-400 hover:text-gray-500 focus:outline-none"
//               >
//                 <X className="h-5 w-5" />
//               </button>
//             </div>
//           </div>
//         ))}
//       </div>
//     </div>
//   );
// };

// export default ServiceNotifications;
