import { useSession } from "../../context/SessionContext";
import NewSessionDialog from "./NewSessionDialog";
import AddTableDialog from "./AddTableDialog";
import NewBillDialog from "./NewBillDialog";

const DialogContainer = () => {
  const { dialogState, closeDialog } = useSession();
  console.log(dialogState);

  return (
    <>
      <NewSessionDialog
        open={dialogState?.newSession || false}
        onClose={() => closeDialog("newSession")}
      />

      <AddTableDialog
        open={dialogState?.addTable || false}
        sessionId={dialogState?.currentSessionId}
        onClose={() => closeDialog("addTable")}
      />

      <NewBillDialog
        open={dialogState?.newBill || false}
        sessionId={dialogState?.currentSessionId}
        onClose={() => closeDialog("newBill")}
      />
    </>
  );
};

export default DialogContainer;
