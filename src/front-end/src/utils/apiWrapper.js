/**
 * Creates a reusable API wrapper that handles loading states, errors, and success callbacks.
 * This eliminates the repetitive try-catch-finally blocks in API hooks.
 * 
 * @param {Object} config - Configuration object
 * @param {Function} config.setIsLoading - Function to set loading state
 * @param {Function} config.setActionError - Function to set error state
 * @param {Function} config.handleError - Function to handle errors
 * @param {Function} config.onSuccess - Success callback (optional)
 * @returns {Function} Wrapper function for API calls
 */
export const createApiWrapper = ({ 
  setIsLoading, 
  setActionError, 
  handleError, 
  onSuccess 
}) => {
  /**
   * Wraps an API call with standard error handling and loading states.
   * 
   * @param {string} context - Name of the operation (for error logging)
   * @param {Function} apiCall - Async function that performs the API call
   * @param {Object} options - Optional configuration
   * @param {boolean} options.triggerSuccess - Whether to call onSuccess callback
   * @param {*} options.defaultReturn - Value to return on error (default: null)
   * @returns {Promise} Result of the API call
   */
  return async (context, apiCall, options = {}) => {
    const { triggerSuccess = false, defaultReturn = null } = options;

    try {
      setActionError(null);
      setIsLoading(true);
      
      const result = await apiCall();
      
      if (triggerSuccess && typeof onSuccess === "function") {
        await onSuccess();
      }
      
      return result;
    } catch (err) {
      handleError(err, context);
      return defaultReturn;
    } finally {
      setIsLoading(false);
    }
  };
};