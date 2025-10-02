import styled from "styled-components";

export const InsightContainer = styled.div`
  animation: fadeIn 2s ease-in-out;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background-color: #f9f9f9;
  margin-top: 20px;
  white-space: pre-wrap;
`;

export const InsightText = styled.p`
  padding: 10px 0;
`;

export const Accordion = styled.div`
  margin-top: 20px;
`;

export const AccordionHeader = styled.div`
  cursor: pointer;
  padding: 10px;
  background-color: #eee;
  border: 1px solid #ddd;
  border-radius: 8px;
`;

export const AccordionContent = styled.div`
  max-height: ${({ isOpen }) => (isOpen ? "1000px" : "0")};
  overflow: hidden;
  transition: max-height 0.3s ease;
`;

export const ProgressBar = styled.div`
  width: 100%;
  background-color: #f3f3f3;
  border-radius: 8px;
  overflow: hidden;
  margin-top: 10px;
`;

export const Progress = styled.div`
  width: ${({ $progress }) => $progress}%;
  height: 10px;
  background-color: #ff0000;
  transition: width 0.3s;
`;
