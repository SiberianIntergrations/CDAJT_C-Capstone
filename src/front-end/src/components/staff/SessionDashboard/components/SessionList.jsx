import { useState, useEffect, useCallback, useRef } from "react";
import { Box, Button } from "@mui/material";
import { useSession } from "../context/SessionContext";
import SessionCard from "./SessionCard";
import api from "@/config/api";

const SessionList = () => {
  const { sessions, openDialog } = useSession();
  const [orderedSessions, setOrderedSessions] = useState([]);
  const [sessionRequests, setSessionRequests] = useState({});
  const [expandedSessionId, setExpandedSessionId] = useState(null);
  const sessionRefs = useRef({});
  const scrollTimeoutRef = useRef(null);

  const fetchSessionRequests = useCallback(async (sessionId) => {
    try {
      // TODO: ServiceRequest endpoint by session ID
      console.log("sessionID: ", sessionId);
      const response = await api.get(`/ServiceRequest/by-session/${sessionId}`);
      if (response.status !== 200) return 0;
      const data = response.data;
      return data.length;
    } catch (error) {
      console.error("Error fetching requests:", error);
      return 0;
    }
  }, []);

  const scrollToSession = useCallback((sessionId) => {
    if (scrollTimeoutRef.current) {
      clearTimeout(scrollTimeoutRef.current);
    }

    scrollTimeoutRef.current = setTimeout(() => {
      const element = sessionRefs.current[sessionId];
      if (element) {
        const viewportHeight = window.innerHeight;
        const elementTop =
          element.getBoundingClientRect().top + window.pageYOffset;

        const scrollPosition =
          elementTop - viewportHeight / 2 + element.offsetHeight / 2;

        window.scrollTo({
          top: scrollPosition,
          behavior: "smooth",
        });
      }
    }, 100);
  }, []);

  // TODO: Consider debouncing with useRef to avoid excessive calls. Look over dependencies
  const updateSessionOrder = useCallback(async () => {
    const requests = {};
    console.log("Sessions: ", sessions);
    await Promise.all(
      sessions.map(async (session) => {
        requests[session.session_Id] = await fetchSessionRequests(
          session.session_Id
        );
      })
    );

    setSessionRequests(requests);

    const ordered = [...sessions].sort((a, b) => {
      const requestsA = requests[a.session_Id] || 0;
      const requestsB = requests[b.session_Id] || 0;
      if (requestsB !== requestsA) {
        return requestsB - requestsA;
      }
      return b.session_Id - a.session_Id;
    });

    setOrderedSessions(ordered);

    if (expandedSessionId) {
      const expandedSessionIndex = ordered.findIndex(
        (s) => s.session_Id === expandedSessionId
      );
      if (expandedSessionIndex !== -1) {
        scrollToSession(expandedSessionId);
      }
    }
  }, [sessions, expandedSessionId, scrollToSession, fetchSessionRequests]); // Removed orderedSessions

  const handleSessionExpand = useCallback(
    (sessionId, isExpanded) => {
      setExpandedSessionId(isExpanded ? sessionId : null);
      if (isExpanded) {
        scrollToSession(sessionId);
      }
    },
    [scrollToSession]
  );

  const handleRequestUpdate = useCallback((sessionId, requests) => {
    setSessionRequests((prev) => ({
      ...prev,
      [sessionId]: requests.length,
    }));
  }, []);

  useEffect(() => {
    return () => {
      if (scrollTimeoutRef.current) {
        clearTimeout(scrollTimeoutRef.current);
      }
    };
  }, []);

  useEffect(() => {
    updateSessionOrder();
    const interval = setInterval(updateSessionOrder, 5000);
    return () => clearInterval(interval);
  }, [updateSessionOrder]);

  return (
    <>
      {orderedSessions.map((session) => (
        <Box
          key={session.session_Id}
          ref={(el) => {
            if (el) {
              sessionRefs.current[session.session_Id] = el;
            }
          }}
        >
          <SessionCard
            session={session}
            onRequestUpdate={handleRequestUpdate}
            expanded={expandedSessionId === session.session_Id}
            onExpand={(isExpanded) =>
              handleSessionExpand(session.session_Id, isExpanded)
            }
          />
        </Box>
      ))}
    </>
  );
};

export default SessionList;
