namespace Server.Game
{
	public static class NetCharacterBuilder
	{
		public static NetObject CreateShelly(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Character_Shelly);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.5f);
			obj.AddComponent<NCharacterShelly>();
			return obj;
		}

		public static NetObject CreateSpike(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Character_Spike);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.5f);
			obj.AddComponent<NCharacterSpike>();
			return obj;
		}
	}
}
