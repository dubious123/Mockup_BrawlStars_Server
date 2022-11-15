global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Net.Sockets;
global using System.Numerics;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Storage;

global using Server.DB.Entities;

#region ServerCore
global using ServerCore;
global using ServerCore.Managers;

global using static ServerCore.Utils.Enums;
global using static ServerCore.Utils.Tools;

#endregion
#region Server
global using Server.DB;
global using Server.Game;
global using Server.Utils;
global using Server.Utils.JsonConverter;

global using static Server.Utils.Enums;
global using static Enums;
#endregion

