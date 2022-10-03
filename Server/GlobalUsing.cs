global using System;
global using System.Diagnostics;
global using System.Linq;
global using System.Text;
global using System.Numerics;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.ComponentModel.DataAnnotations;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Storage;
global using Server.DB.Entities;
global using System.Threading;
global using System.IO;
global using System.Net.Sockets;
#region ServerCore
global using ServerCore;
global using ServerCore.Managers;
global using ServerCore.Utils;
global using static ServerCore.Utils.Tools;
global using static ServerCore.Utils.Enums;
#endregion
#region Server
global using Server.DB;
global using Server.Log;
global using Server.Utils;
global using Server.Game;
global using Server.Game.Managers;
global using Server.Game.Base;
global using Server.Game.Base.Utils;
global using Server.Game.Characters.Base.Skill;
global using Server.Utils.JsonConverters;
global using static Server.Utils.Enums;
#endregion
