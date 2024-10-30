The TCP command is as below, they are supposed to be sent from client side in TCP communication
REQUEST - To get the info of whether the pcb is defected.
JOBDONE - To tell the middleware side pcb is processed.

Both of the command above need to attached with a number at the end which is the index number of the pcb. for example: REQUEST + 13, convert them into byte array become 52 45 51 55 45 53 54 0D.
The command sent from the client side should be always of above two, otherwise the server will only send back "RCVERROR"

There are few possible reply from server
RCVERROR                              
Send back from server if the command from client is not recognized.

REQUEST + pcb number + 1 or 0         
Note 1 or 0 here is to tell if there is defected, 1 means defected.

SUCCESS + pcb number                  
Send back from server after receive JOBDONE from client, if the relevant pcb number is processed and deleted.

NOTRQST +pcb number                   
Send back from server after receive JOBDONE from client, if the relevant pcb number is yet to be processed.

NOTEXIST                             
Send back from the server if the pcb number is not exist.
