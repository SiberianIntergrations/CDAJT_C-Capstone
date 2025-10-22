import { Box, Typography, Tabs, Tab } from "@mui/material";
import { useState } from "react";
import { useTable } from "../context/TableContext";
import TableCard from "./TableCard";
import TableGroupCard from "./TableGroupCard";

const TableList = () => {
  const { tables, tableGroups } = useTable();
  const [activeTab, setActiveTab] = useState(0);

  return (
    <Box>
      <Tabs
        value={activeTab}
        onChange={(e, val) => setActiveTab(val)}
        sx={{ mb: 2 }}
      >
        <Tab label="Individual Tables" />
        <Tab label="Table Groups" />
      </Tabs>

      {activeTab === 0 && (
        <Box>
          {tables.length === 0 ? (
            <Typography
              variant="body1"
              color="text.secondary"
              textAlign="center"
            >
              No tables found
            </Typography>
          ) : (
            tables.map((table) => (
              <TableCard key={table.table_Id} table={table} />
            ))
          )}
        </Box>
      )}

      {activeTab === 1 && (
        <Box>
          {tableGroups.length === 0 ? (
            <Typography
              variant="body1"
              color="text.secondary"
              textAlign="center"
            >
              No table groups found
            </Typography>
          ) : (
            tableGroups.map((group) => (
              <TableGroupCard key={group.tableGroup_Id} group={group} />
            ))
          )}
        </Box>
      )}
    </Box>
  );
};

export default TableList;
