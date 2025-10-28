import { useMemo } from "react";
import { Box } from "@mui/material";
import TagChip from "./tags/TagChip";


const Tags = ({ tags = [], size = "l" }) => {
  const normalized = useMemo(() => {
    return (Array.isArray(tags) ? tags : []).map((t, idx) => ({
      tag_id: t.tag_id ?? t.Tag_Id ?? t.id ?? `${t.name ?? t.tag_name ?? idx}`,
      name: t.name ?? t.tag_name ?? "",
      color_code: t.color_code ?? t.color ?? t.colorCode ?? undefined,
    }));
  }, [tags]);

  if (!normalized.length) return null;

  return (
    <Box
      sx={{
        display: "flex",
        flexWrap: "wrap",
        gap: 0.5,
        mt: 0.5,
        maxWidth: "100%",
      }}
    >
      {normalized.map((tag) => (
        <TagChip key={tag.tag_id} tag={tag} size={size} />
      ))}
    </Box>
  );
};

export default Tags;
