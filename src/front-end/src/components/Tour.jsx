import React from "react";
import Joyride from "react-joyride";

const Tour = ({ run, setRun, stepIndex, setStepIndex, callback, steps }) => {
  return (
    <Joyride
      steps={steps}
      run={run}
      stepIndex={stepIndex}
      continuous
      showSkipButton
      showProgress
      disableScrolling
      disableOverlayClose= {true}
      styles={{
        options: {
          zIndex: 2000,
        },
      }}
      callback={callback}
    />
  );
};

export default Tour;
