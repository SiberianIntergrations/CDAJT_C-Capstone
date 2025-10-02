// File: sushi-toshi-frontend/components/Tags.jsx
import { useEffect, useState } from 'react';
import { Box } from '@mui/material';
import TagChip from './tags/TagChip';
import axios from 'axios';
import { axiosInstance, createApiUrl } from '../config/api';

const Tags = ({ item_id, size='l' }) => {
  const [tags, setTags] = useState([]);
  const [error, setError] = useState(null);

  const getToken = () => {
    if (typeof window !== "undefined") {
      return localStorage.getItem("access_token");
    }
    return null;
  };

  useEffect(() => {
    const fetchTags = async () => {
      try {
        const token = getToken();
        if (!token) {
          setError('No authentication token found');
          return;
        }

        const response = await axiosInstance.get(
          `/menu-items/${item_id}/tags-with-colors`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.data) {
          setTags(response.data);
          setError(null);
        }
      } catch (error) {
        console.error('Error fetching tags:', error);
        setError('Failed to load tags');
      }
    };

    if (item_id) {
      fetchTags();
    }
  }, [item_id]);

  if (error) return null; 
  if (!tags.length) return null;

  return (
    <Box
      sx={{
        display: 'flex',
        flexWrap: 'wrap',
        gap: 0.5,
        mt: 0.5,
        maxWidth: '100%'
      }}
    >
      {tags.map((tag) => (
        <TagChip 
          key={tag.tag_id} 
          tag={tag} 
          size={size}
        />
      ))}
    </Box>
  );
};

export default Tags;