using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.Models;

namespace SignalR.API.Hubs
{
    public class MyHub : Hub
    {
        private readonly Context _context;

        public MyHub(Context context)
        {
            _context = context;
        }

        public static List<string> Names { get; set; } = new List<string>();

        public static int ClientCount { get; set; } = 0;
        public static int RoomCount { get; set; } = 5;

        public async Task SendName(string name)
        {
            Names.Add(name);
            await Clients.All.SendAsync("RecieveName", name);
        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("RecieveNames", Names);
        }

        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("RecieveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("RecieveClientCount", ClientCount);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNameByGroup(string userName, string roomName)
        {
            var room = _context.Rooms.Where(x => x.RoomName == roomName).FirstOrDefault();
            if (room != null)
            {
                room.Users.Add(new User { UserName = userName });
            }
            else
            {
                var newRoom = new Room { RoomName = roomName };
                newRoom.Users.Add(new User { UserName = userName });
                _context.Rooms.Add(newRoom);
            }

            await _context.SaveChangesAsync();
            await Clients.Group(roomName).SendAsync("ReceiveMessageByGroup", userName, room.RoomID);
        }

        public async Task GetNamesByGroup()
        {
            var rooms = _context.Rooms.Include(x => x.Users).Select(y => new
            {
                roomID = y.RoomID,
                users = y.Users.ToList()
            });

            await Clients.All.SendAsync("ReceiveNamesByGroup", rooms);
        }

        public async Task AddToGroup(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task RemoveToGroup(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}