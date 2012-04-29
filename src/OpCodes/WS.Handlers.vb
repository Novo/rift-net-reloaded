Public Module WS_Handlers

    Public Sub IntializeWorldPacketHandlers()
        'PacketHandlers(OpCodes.CMSG_DONT_KNOW) = CType(AddressOf OnUnhandledPacket, HandlePacket)

        PacketHandlers(OpCodes.CMSG_PING) = CType(AddressOf On_CMSG_PING, HandlePacket)

        PacketHandlers(OpCodes.CMSG_AUTH_SESSION) = CType(AddressOf On_CMSG_AUTH_SESSION, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_ENUM) = CType(AddressOf On_CMSG_CHAR_ENUM, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_CREATE) = CType(AddressOf On_CMSG_CHAR_CREATE, HandlePacket)
        PacketHandlers(OpCodes.CMSG_CHAR_DELETE) = CType(AddressOf On_CMSG_CHAR_DELETE, HandlePacket)
        PacketHandlers(OpCodes.CMSG_PLAYER_LOGIN) = CType(AddressOf On_CMSG_PLAYER_LOGIN, HandlePacket)
    End Sub


    Public Sub OnUnhandledPacket(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("{0} {1} [Unhandled World Packet]", CType(packet.OpCode, OpCodes))
    End Sub

End Module