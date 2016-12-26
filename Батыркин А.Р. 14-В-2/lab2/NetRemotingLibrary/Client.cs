using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteBase
{
    public class Client
    {
        public enum ClientStatus { BUSY, FREE }

        private int id = 0;
        private ClientStatus status = ClientStatus.FREE;
        private bool hadAdminRights = false;
        private DateTime lastInteraction = DateTime.Now;
        private int meta = 0;

        public Client(int clientID)
        {
            id = clientID;
            status = ClientStatus.FREE;
            hadAdminRights = false;
        }

        public void SetAdmin(bool admin)
        {
            hadAdminRights = admin;
        }

        public bool GetAdmin()
        {
            return hadAdminRights;
        }

        public int GetID()
        {
            return id;
        }

        public void SetStatus(ClientStatus clientStatus)
        {
            status = clientStatus;
            lastInteraction = DateTime.Now;
        }

        public DateTime GetTimeSinceLastInteraction()
        {
            return lastInteraction;
        }

        public ClientStatus GetStatus()
        {
            return status;
        }

        public int GetMeta()
        {
            return meta;
        }

        public void SetMeta(int clientMeta)
        {
            meta = clientMeta;
        }
    }
}
