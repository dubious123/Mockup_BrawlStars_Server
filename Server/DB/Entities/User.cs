using System;
using System.ComponentModel.DataAnnotations;
using static Server.Utils.Enums;

namespace Server.DB.Entities
{
	public class User
	{
		[Key]
		public int UserId { get; set; }
		public DateTime CreatedTime { get; set; }
		public DateTime? DeletedTime { get; set; }
		public string LoginId { get; set; }
		public string LoginPw { get; set; }
		public CharacterType LastSelectedCharacterType { get; set; }
	}
}
