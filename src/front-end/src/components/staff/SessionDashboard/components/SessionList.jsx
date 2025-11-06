import { useState, useEffect, useCallback, useRef } from "react";
import { Box } from "@mui/material";
import { useSession } from "../context/SessionContext";
import SessionCard from "./SessionCard";

const SessionList = () => {
  const { sessions } = useSession();
  const [orderedSessions, setOrderedSessions] = useState([]);
  const [sessionRequests, setSessionRequests] = useState({});
  const [expandedSessionId, setExpandedSessionId] = useState(null);
  const sessionRefs = useRef({});
  const scrollTimeoutRef = useRef(null);

  const scrollToSession = useCallback((sessionId) => {
    if (scrollTimeoutRef.current) {
      clearTimeout(scrollTimeoutRef.current);
    }

    scrollTimeoutRef.current = setTimeout(() => {
      const element = sessionRefs.current[sessionId];
      if (element) {
        const viewportHeight = window.innerHeight;
        const elementTop =
          element.getBoundingClientRect().top + window.scrollY;

        const scrollPosition =
          elementTop - viewportHeight / 2 + element.offsetHeight / 2;

        window.scrollTo({
          top: scrollPosition,
          behavior: "smooth",
        });
      }
    }, 100);
  }, []);

  useEffect(() => {
    const sessionList = sessions || [];
    const ordered = [...sessionList].sort((a, b) => {
      const requestsA = sessionRequests[a.session_Id] || 0;
      const requestsB = sessionRequests[b.session_Id] || 0;
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
  }, [sessions, sessionRequests, expandedSessionId, scrollToSession]);

  const handleSessionExpand = useCallback(
    (sessionId, isExpanded) => {
      setExpandedSessionId(isExpanded ? sessionId : null);
      if (isExpanded) {
        scrollToSession(sessionId);
      }
    },
    [scrollToSession]
  );

  // Handle request count updates from SessionCard
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

  return (
    <>
      {orderedSessions.map((session) => (
        <Box
          key={session.session_Id}
          ref={(el) => {
            if (el) {
              sessionRefs.current[session.session_Id] = el;
            } else {
              // Clean up ref when element is unmounted
              delete sessionRefs.current[session.session_Id];
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
