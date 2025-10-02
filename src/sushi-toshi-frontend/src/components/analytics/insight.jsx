import React, { useState } from "react";
import { GoogleGenerativeAI } from "@google/generative-ai";
import styles from "@/components/analytics/insight.module.css";

const apiKey = process.env.NEXT_PUBLIC_OPENAI_API_KEY;
const genAI = new GoogleGenerativeAI(apiKey);
const model = genAI.getGenerativeModel({ model: "gemini-1.5-flash" });

import { Grid } from "@mui/material";

const InsightTextBox = ({ topItems, bottomItems, question }) => {
  const [inputText, setInputText] = useState(question);
  const [insightText, setInsightText] = useState("");
  const [isAccordionOpen, setIsAccordionOpen] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const [progress, setProgress] = useState(0);

  const fetchInsight = async (retryCount = 0) => {
    setIsLoading(true);
    setProgress(0);

    const interval = setInterval(() => {
      setProgress((prev) => (prev < 100 ? prev + 10 : prev));
    }, 300);

    const topItemsText = topItems.map((item) => item.name).join(", ");
    const bottomItemsText = bottomItems.map((item) => item.name).join(", ");
    const fullPrompt = `${question}\nTop 5 Items: ${topItemsText}\nBottom 5 Items: ${bottomItemsText}`;

    try {
      const result = await model.generateContent(fullPrompt);
      setInsightText(result.response.text());
      setProgress(100); // Set progress to 100% when fetch is complete
      setIsLoading(false);
    } catch (error) {
      console.error("Error fetching insight:", error);
      if (error.response) {
        console.error("Error response data:", error.response.data);
        if (error.response.status === 401) {
          console.error("Unauthorized: Check your API key.");
        } else if (error.response.status === 403) {
          console.error("Forbidden: You do not have access to this resource.");
        } else if (error.response.status === 404) {
          console.error("Not Found: Check the API endpoint.");
        } else if (error.response.status === 400) {
          console.error("Bad Request: Check the request parameters.");
          console.error("Request data:", {
            model: "gemini-1.5-flash",
            prompt: fullPrompt,
            max_tokens: 100,
            temperature: 0.7,
            n: 1,
            stop: null,
          });
          console.error("Response data:", error.response.data);
        } else if (error.response.status === 429) {
          console.error("Too Many Requests: You have hit the rate limit.");
          if (retryCount < 3) {
            console.log(`Retrying... (${retryCount + 1})`);
            setTimeout(
              () => fetchInsight(retryCount + 1),
              1000 * Math.pow(2, retryCount)
            );
          }
        }
      }
      setProgress(1);
      setIsLoading(false);
    } finally {
      clearInterval(interval);
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    fetchInsight();
  };

  const toggleAccordion = () => {
    setIsAccordionOpen(!isAccordionOpen);
  };

  return (
    <Grid container spacing={2}>
      <Grid item xs={12}>
        <form onSubmit={handleSubmit}>
          <Grid item xs={12}>
            <button
              className={styles.styledButton}
              type="submit"
              style={{
                marginBottom: "20px",
                marginTop: "20px",
                fontSize: "20px",
                padding: "10px 20px",
              }}
            >
              AI Business Analyzer
            </button>
            <span role="img" aria-label="robot">
              🤖 Click this button for some business suggestions...limits per
              day
            </span>
          </Grid>
          {isLoading && (
            <Grid item xs={12}>
              <div className={styles.progressBar}>
                <div
                  className={styles.progress}
                  style={{ width: `${progress}%`, backgroundColor: "red" }}
                />
              </div>
            </Grid>
          )}
        </form>
      </Grid>
      {insightText && (
        <Grid item xs={12}>
          <div className={styles.accordion}>
            <div className={styles.accordionHeader} onClick={toggleAccordion}>
              <h3>Insight:</h3>
            </div>
            <div
              className={styles.accordionContent}
              style={{ maxHeight: isAccordionOpen ? "1000px" : "0" }}
            >
              <div className={styles.insightContainer}>
                <p className={styles.insightText}>{insightText}</p>
              </div>
            </div>
          </div>
        </Grid>
      )}
    </Grid>
  );
};

export default InsightTextBox;
