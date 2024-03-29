using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerContactsAdd)]
    public class ServerContactsAdd : IWritable
    {
        //public ContactData Contact { get; set; } = new ContactData();

        public void Write(GamePacketWriter writer)
        {
            //Contact.Write(writer);
        }
    }
}