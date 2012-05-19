Imports System.Data

Public Module WS_Handlers_Auth

    Public Sub On_CMSG_WORLD_TELEPORT(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        packet.ReadUInt32() 'dunno what this is
        Dim zone As Byte = packet.ReadUInt8()
        Dim x As Single = packet.ReadFloat()
        Dim y As Single = packet.ReadFloat()
        Dim z As Single = packet.ReadFloat()
        Dim o As Single = packet.ReadFloat()

        Dim movementStatus As New PacketWriter(OpCodes.SMSG_MOVE_WORLDPORT_ACK)
        movementStatus.WriteUInt64(0)
        movementStatus.WriteSingle(0)
        movementStatus.WriteSingle(0)
        movementStatus.WriteSingle(0)
        movementStatus.WriteSingle(0)
        movementStatus.WriteSingle(x)
        movementStatus.WriteSingle(y)
        movementStatus.WriteSingle(z)
        movementStatus.WriteSingle(o)
        movementStatus.WriteSingle(0)
        movementStatus.WriteUInt32(&H8000000)
        Client.SendWorldClient(movementStatus)
    End Sub


    Public Sub On_MSG_MOVE_WORLDPORT_ACK(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        '
    End Sub


    Public Sub On_CMSG_CANCEL_TRADE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim response As New PacketWriter(OpCodes.SMSG_TRADE_STATUS)
        response.WriteUInt32(TradeStatus.TRADE_STATUS_CANCELLED)

        Client.SendWorldClient(response)
    End Sub


    Public Sub On_CMSG_QUERY_TIME(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim response As New PacketWriter(OpCodes.SMSG_QUERY_TIME_RESPONSE)
        response.WriteInt32(System.Environment.TickCount) 'this is not correct?

        Client.SendWorldClient(response)
    End Sub


    Public Sub On_HandleMovementStatus(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        'Read MovementStatus
        Dim MoveMessage As OpCodes = packet.Opcode

        Dim TransportGuid As UInt64 = packet.ReadUInt64()
        Dim TransportX As Single = packet.ReadFloat()
        Dim TransportY As Single = packet.ReadFloat()
        Dim TransportZ As Single = packet.ReadFloat()
        Dim TransportO As Single = packet.ReadFloat()
        Dim X As Single = packet.ReadFloat()
        Dim Y As Single = packet.ReadFloat()
        Dim Z As Single = packet.ReadFloat()
        Dim O As Single = packet.ReadFloat()
        Dim Pitch As Single = packet.ReadFloat()
        Dim Flags As UInt32 = packet.ReadUInt32()

        'Send MovementStatus
        Dim movementStatus As New PacketWriter(MoveMessage)

        movementStatus.WriteUInt64(TransportGuid)
        movementStatus.WriteSingle(TransportX)
        movementStatus.WriteSingle(TransportY)
        movementStatus.WriteSingle(TransportZ)
        movementStatus.WriteSingle(TransportO)
        movementStatus.WriteSingle(X)
        movementStatus.WriteSingle(Y)
        movementStatus.WriteSingle(Z)
        movementStatus.WriteSingle(O)
        movementStatus.WriteSingle(Pitch)
        movementStatus.WriteUInt32(Flags)

        Client.SendWorldClient(movementStatus)

        'Save MovementStatus
        Client.Character.PositionX = X
        Client.Character.PositionY = Y
        Client.Character.PositionZ = Z
        Client.Character.Facing = O


        'ToDo: Save in DB every x seconds
    End Sub


    Public Sub On_CMSG_MESSAGECHAT(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim response As New PacketWriter(OpCodes.SMSG_MESSAGECHAT)

        response.WriteUInt8(CByte(packet.ReadInt32)) ' slashCmd, 9: SystemMessage
        response.WriteUInt32(packet.ReadUInt32)      ' Language: General
        response.WriteUInt64(0)    ' Guid: 0 - ToAll???
        response.WriteString(packet.ReadString)
        response.WriteUInt8(0)     'afkDND, 0: nothing

        Client.SendWorldClient(response)
    End Sub


    Public Sub On_CMSG_LOGOUT_REQUEST(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        'Dim response As New PacketWriter(OpCodes.SMSG_LOGOUT_RESPONSE)
        'Client.SendWorldClient(response)

        'Save Character
        Dim Success As Boolean = False
        Success = Client.Character.SaveToDatabase()


        If Success Then
            Console.WriteLine("Character saved!")

            Dim logout_complete As New PacketWriter(OpCodes.SMSG_LOGOUT_COMPLETE)
            Client.SendWorldClient(logout_complete)
        Else
            Console.WriteLine("Character could not be saved, abort logout!")
        End If

    End Sub


    Public Sub On_CMSG_NAME_QUERY(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        'Read GUID
        Client.Character.GUID = CUInt(packet.ReadUInt64)

        'Read from Database
        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name, race, class, gender FROM characters WHERE GUID = " & Client.Character.GUID.ToString)
        CharDB.DisposeDatabaseConnection()

        'Save into Character Object
        Client.Character.Name = DirectCast(result.Rows(0).ItemArray(0), String)
        Client.Character.Race = CType(result.Rows(0).ItemArray(1), GlobalConstants.Races)
        Client.Character.Classe = CType(result.Rows(0).ItemArray(2), GlobalConstants.Classes)
        Client.Character.Gender = CType(result.Rows(0).ItemArray(3), GlobalConstants.Genders)


        Dim response As New PacketWriter(OpCodes.SMSG_NAME_QUERY_RESPONSE)

        response.WriteUInt64(Client.Character.GUID) 'player guid
        response.WriteString(Client.Character.Name) 'player name
        response.WriteUInt32(Client.Character.Race) 'race
        response.WriteUInt32(Client.Character.Gender) 'gender
        response.WriteUInt32(Client.Character.Classe) 'class
        response.WriteUInt8(0) '// tell the server this name is not declined...

        Client.SendWorldClient(response)
    End Sub


    Public Sub On_CMSG_PLAYER_LOGIN(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        'Temp: Send World Server down
        'Dim down As New PacketWriter(OpCodes.SMSG_CHARACTER_LOGIN_FAILED)
        'down.WriteUInt8(AuthLoginCodes.CHAR_LOGIN_DISABLED)
        'Client.SendWorldClient(down)
        'Console.WriteLine("[{0}] Unable to login: WORLD SERVER DOWN", Format(TimeOfDay, "HH:mm:ss"), Client.WSIP, Client.WSPort)
        'Exit Sub
        '----------------------------




        'SMSG_LEARNED_SPELL
        '0x011E 	OnLearnedSpell 	SMSG_LEARNED_SPELL
        'ushort    spell ID
        'short    slot
        Dim x As New PacketWriter(OpCodes.SMSG_LEARNED_SPELL)

        x.WriteUInt16(&H12) ' Spell ID
        x.WriteInt16(1)     ' Spell Slot (positive for spells, negative for abilities)

        Client.SendWorldClient(x)



        'SMSG_INITIAL_SPELLS
        '0x011D 	OnInitialSpells 	SMSG_INITIAL_SPELLS
        'uchar(unknown)
        'ushort    count
        'Loop:
        '  ushort    spell/ability id
        '  short    spell/ability slot (positive for spells, negative for abilities)
        'ushort count
        'Loop:
        '  ushort
        '  ushort
        '  ushort
        '  Int()
        '  Int()

        'Dim test As New PacketWriter(OpCodes.SMSG_INITIAL_SPELLS)

        'test.WriteUInt8(0)      ' unknown

        'test.WriteUInt16(2)     ' Spell count

        ''Spell 1
        'test.WriteUInt16(&H12)  ' Spell ID
        'test.WriteUInt16(1)     ' Spell Slot (positive for spells, negative for abilities)

        ''Spell 2
        'test.WriteUInt16(&H74)  ' Spell ID
        'test.WriteUInt16(2)     ' Spell Slot (positive for spells, negative for abilities)

        'test.WriteUInt16(0)     ' Spell count

        'Client.SendWorldClient(test)





        Dim response As New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)
        Client.SendWorldClient(WS_Handlers_World.Build_SMSG_UPDATE_OBJECT(packet, response, Client))



        'FAIL? Need Chat Handler ...
        Dim chat_welcome As New PacketWriter(OpCodes.SMSG_MESSAGECHAT)

        chat_welcome.WriteUInt8(9)     ' slashCmd, 9: SystemMessage
        chat_welcome.WriteUInt32(0)    ' Language: General
        chat_welcome.WriteUInt64(0)    ' Guid: 0 - ToAll???
        chat_welcome.WriteString("=====================================" & Environment.NewLine & "Welcome to: " & CType(System.Reflection.[Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyTitleAttribute), False)(0), System.Reflection.AssemblyTitleAttribute).Title & " Version: " & System.Reflection.[Assembly].GetExecutingAssembly().GetName().Version.ToString & Environment.NewLine & "Type !help for server command help." & Environment.NewLine & "=====================================")
        chat_welcome.WriteUInt8(0)     'afkDND, 0: nothing

        Client.SendWorldClient(chat_welcome)


        Dim chat_player As New PacketWriter(OpCodes.SMSG_MESSAGECHAT)

        chat_player.WriteUInt8(9)     ' slashCmd, 9: SystemMessage
        chat_player.WriteUInt32(0)    ' Language: General
        chat_player.WriteUInt64(0)    ' Guid: 0 - ToAll???
        chat_player.WriteString("Player: " & Client.Character.Name & " logged in.")
        chat_player.WriteUInt8(0)     'afkDND, 0: nothing

        Client.SendWorldClient(chat_player)


        'ToDo:
        '/* send SMSG_FRIEND_LIST */
        '/* send SMSG_IGNORE_LIST */
        '/* send SMSG_BINDPOINTUPDATE */
        '/* send SMSG_TUTORIAL_FLAGS */
        '/* send SMSG_INITIAL_SPELLS */
        '/* send SMSG_ACTION_BUTTONS */
        '/* send SMSG_INITIALIZE_FACTIONS */
        '/* send SMSG_LOGIN_SETTIMESPEED */
        '/* Introduce the new player to the enviroment and vica versa */
    End Sub


    Public Sub On_CMSG_CHAR_DELETE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_DELETE)

        Try
            Client.Character.GUID = packet.ReadUInt64

            Dim success As Boolean = False
            Dim CharDB As New SQLiteBase("characterDB")

            success = CharDB.Execute("DELETE from characters WHERE guid = " & Client.Character.GUID.ToString)


            If success Then
                Console.WriteLine("[{0}] [{1}:{2}] Player GUID: {1} deleted...", Format(TimeOfDay, "HH:mm:ss"), Client.Character.GUID, Client.WSIP, Client.WSPort)
            Else
                Console.WriteLine("[{0}] [{1}:{2}] Player GUID: {1} deletion FAILED!", Format(TimeOfDay, "HH:mm:ss"), Client.Character.GUID, Client.WSIP, Client.WSPort)
            End If


            Client.SendWorldClient(response)

            CharDB.DisposeDatabaseConnection()

        Catch ex As Exception
            Console.WriteLine("[{0}] [{1}:{2}] Player GUID: {1} deletion FAILED!", Format(TimeOfDay, "HH:mm:ss"), Client.Character.GUID, Client.WSIP, Client.WSPort)
        End Try

    End Sub


    Public Function HandleCharCreate(ByRef packet As PacketReader, ByRef response As PacketWriter, ByRef Client As WorldServerClass) As PacketWriter

        'Get Account ID
        Dim AccountID As Integer = 0
        Dim realmDB As New SQLiteBase("realmDB")

        Dim realmDBTable As DataTable = realmDB.Select("SELECT account_id FROM accounts WHERE username = '" & Client.Character.clientAccount & "'")
        realmDB.DisposeDatabaseConnection()

        Dim CountrealmDBTable As Integer = realmDBTable.Rows.Count

        AccountID = CInt(realmDBTable.Rows(0).ItemArray(0))


        'Read Character Data from Client
        Client.Character.Name = packet.ReadString()
        Client.Character.Race = CType(packet.ReadByte(), GlobalConstants.Races)
        Client.Character.Classe = CType(packet.ReadByte(), GlobalConstants.Classes)
        Client.Character.Gender = CType(packet.ReadByte(), GlobalConstants.Genders)
        Client.Character.Skin = packet.ReadByte()
        Client.Character.Face = packet.ReadByte()
        Client.Character.HairStyle = packet.ReadByte()
        Client.Character.HairColor = packet.ReadByte()
        Client.Character.FacialHair = packet.ReadByte()


        'Save Start Area:
        'DEBUG
        'Client.Character.ZoneID = 1
        'Client.Character.MapID = 0
        'Client.Character.PositionX = 16419.3828
        'Client.Character.PositionY = 16243.0635
        'Client.Character.PositionZ = 70.43047
        'Client.Character.Facing = 0.6499118



        Select Case Client.Character.Race
            Case GlobalConstants.Races.RACE_HUMAN
                Client.Character.ZoneID = 12
                Client.Character.MapID = 0
                Client.Character.PositionX = -8942.044
                Client.Character.PositionY = -130.8001
                Client.Character.PositionZ = 84.83379
                Client.Character.Facing = 0.5806077
                Exit Select

            Case GlobalConstants.Races.RACE_ORC
                Client.Character.ZoneID = 14
                Client.Character.MapID = 1
                Client.Character.PositionX = -618.518005
                Client.Character.PositionY = -4251.669922
                Client.Character.PositionZ = 38.717999
                Client.Character.Facing = 1
                Exit Select

            Case GlobalConstants.Races.RACE_DWARF
                Client.Character.ZoneID = 1
                Client.Character.MapID = 0
                Client.Character.PositionX = -6240.319824
                Client.Character.PositionY = 331.03299
                Client.Character.PositionZ = 382.757996
                Client.Character.Facing = 1
                Exit Select

            Case GlobalConstants.Races.RACE_NIGHT_ELF
                Client.Character.ZoneID = 141
                Client.Character.MapID = 1
                Client.Character.PositionX = -10311.943359
                Client.Character.PositionY = 832.356689
                Client.Character.PositionZ = 1326.395752
                Client.Character.Facing = 0.210676
                Exit Select

            Case GlobalConstants.Races.RACE_UNDEAD
                Client.Character.ZoneID = 14
                Client.Character.MapID = 1
                Client.Character.PositionX = -618.518005
                Client.Character.PositionY = -4251.669922
                Client.Character.PositionZ = 38.717999
                Client.Character.Facing = 1
                Exit Select

            Case GlobalConstants.Races.RACE_TAUREN
                Client.Character.ZoneID = 215
                Client.Character.MapID = 1
                Client.Character.PositionX = -2917.580078
                Client.Character.PositionY = -257.980011
                Client.Character.PositionZ = 52.996799
                Client.Character.Facing = 1
                Exit Select

            Case GlobalConstants.Races.RACE_GNOME
                Client.Character.ZoneID = 1
                Client.Character.MapID = 0
                Client.Character.PositionX = -6240.319824
                Client.Character.PositionY = 331.03299
                Client.Character.PositionZ = 382.757996
                Client.Character.Facing = 1
                Exit Select

            Case GlobalConstants.Races.RACE_TROLL
                Client.Character.ZoneID = 14
                Client.Character.MapID = 1
                Client.Character.PositionX = -618.518005
                Client.Character.PositionY = -4251.669922
                Client.Character.PositionZ = 38.717999
                Client.Character.Facing = 1
                Exit Select
        End Select


        'Save Character Data into Account
        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name from characters WHERE guid = " & Client.Character.GUID)

        Dim Count As Integer = result.Rows.Count
        For i As Integer = 0 To Count - 1
            If result.Rows(i).ItemArray(0).ToString() = Client.Character.Name Then 'if name already in Database:
                CharDB.DisposeDatabaseConnection() 'close Database
                response.WriteUInt8(CharCreateResponseCodes.NAME_ALREADY_TAKEN)
                Console.WriteLine("[{0}] [{1}:{2}] Player Name: {3} already taken! Account:{4}({5})", Format(TimeOfDay, "HH:mm:ss"), Client.WSIP, Client.WSPort, Client.Character.Name, Client.Character.clientAccount, AccountID)
                Return response 'send response to client
                Exit Function 'exit this function
            End If
        Next

        'if name is not in Database:                '0      '1    '2     '3     '4      '5    '6      '7         '8         '9        '10    '11  '12'13'14   '15
        CharDB.Execute("INSERT INTO characters (account_id, name, race, class, gender, skin, face, hairstyle, haircolor, facialhair, zoneID, mapID, x, y, z, facing) VALUES (" & _
                       "  {0},         '{1}',                      {2},                               {3},                                 {4},                            {5},               {6},               {7},                    {8},                    {9},                    {10},                {11},               {12},                                                                                       {13},                                                                   {14},                                                                             {15})", _
                       AccountID, Client.Character.Name, Convert.ToByte(Client.Character.Race), Convert.ToByte(Client.Character.Classe), Convert.ToByte(Client.Character.Gender), Client.Character.Skin, Client.Character.Face, Client.Character.HairStyle, Client.Character.HairColor, Client.Character.FacialHair, Client.Character.ZoneID, Client.Character.MapID, Client.Character.PositionX.ToString("F", Globalization.CultureInfo.InvariantCulture), Client.Character.PositionY.ToString("F", Globalization.CultureInfo.InvariantCulture), Client.Character.PositionZ.ToString("F", Globalization.CultureInfo.InvariantCulture), Client.Character.Facing.ToString("F", Globalization.CultureInfo.InvariantCulture))

        CharDB.DisposeDatabaseConnection()

        response.WriteUInt8(CharCreateResponseCodes.SUCCESS)
        Return response
    End Function


    Public Sub On_CMSG_CHAR_CREATE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_CREATE)
        Client.SendWorldClient(HandleCharCreate(packet, response, Client))
    End Sub


    Public Function HandleCharEnum(ByVal DBresult As DataTable, ByVal bNumChars As Byte, ByRef response As PacketWriter) As PacketWriter

        For i As Integer = 0 To bNumChars - 1
            response.WriteUInt64(Convert.ToUInt64(DBresult.Rows(i).ItemArray(0))) 'GUID
            response.WriteString(DirectCast(DBresult.Rows(i).ItemArray(1), String)) 'Name

            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(2))) 'Race
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(3))) 'Class
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(4))) 'Gender
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(5))) 'Skin
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(6))) 'Face
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(7))) 'HairStyle
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(8))) 'HairColor
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(9))) 'FacialHair
            response.WriteUInt8(Convert.ToByte(DBresult.Rows(i).ItemArray(10))) 'Level

            response.WriteUInt32(Convert.ToUInt32(DBresult.Rows(i).ItemArray(11))) 'ZoneID
            response.WriteUInt32(Convert.ToUInt32(DBresult.Rows(i).ItemArray(12))) 'MapID

            response.WriteSingle(Convert.ToSingle(DBresult.Rows(i).ItemArray(13))) 'CurX
            response.WriteSingle(Convert.ToSingle(DBresult.Rows(i).ItemArray(14))) 'CurY
            response.WriteSingle(Convert.ToSingle(DBresult.Rows(i).ItemArray(15))) 'CurZ

            response.WriteUInt32(Convert.ToUInt32(DBresult.Rows(i).ItemArray(16))) 'GuildID
            response.WriteUInt32(Convert.ToUInt32(DBresult.Rows(i).ItemArray(17))) 'PetDisplayId
            response.WriteUInt32(Convert.ToUInt32(DBresult.Rows(i).ItemArray(18))) 'PetLevel
            response.WriteUInt32(Convert.ToUInt32(DBresult.Rows(i).ItemArray(19))) 'PetFamilyID

            ' Not implented
            For j As Integer = 1 To 20
                ' DisplayId [uint32]
                ' ItemType [byte]
                response.WriteUInt32(0)
                response.WriteUInt8(0)
            Next

        Next

        Return response
    End Function


    Public Sub On_CMSG_CHAR_ENUM(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim bNumChars As Byte = 0

        'Get Account ID from UserName
        Dim account_id As Integer = 0
        Dim realmDB As New SQLiteBase("realmDB")

        Dim realmDBTable As DataTable = realmDB.Select("SELECT account_id FROM accounts WHERE username = '" & Client.Character.clientAccount & "'")
        realmDB.DisposeDatabaseConnection()

        Dim CountrealmDBTable As Integer = realmDBTable.Rows.Count
        account_id = CInt(realmDBTable.Rows(0).ItemArray(0))

        'Get Character by Account ID
        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT guid, name, race, class, gender, skin, face, hairstyle, " & _
                                                "haircolor, facialhair, level, zoneID, mapID, x, y, z, guildId, petdisplayId, " & _
                                                "petlevel, petfamily FROM characters WHERE account_id = " & account_id)

        bNumChars = CByte(result.Rows.Count)

        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_ENUM)
        response.WriteUInt8(bNumChars) 'Number of Characters
        Client.SendWorldClient(HandleCharEnum(result, bNumChars, response))

        CharDB.DisposeDatabaseConnection()
    End Sub


    Public Sub On_CMSG_AUTH_SESSION(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim clientVersion As Integer = packet.ReadInt32
        Dim clientSesionID As Integer = packet.ReadInt32
        Client.Character.clientAccount = packet.ReadAccountName.Trim.ToUpper
        Dim clientPassword As String = packet.ReadString.Trim


        'Start Sending Auth ...
        ' Dim responseAuth As New PacketWriter(OpCodes.SMSG_AUTH_RESPONSE)
        ' responseAuth.WriteUInt8(AuthResponseCodes.AUTH)
        ' Client.SendWorldClient(responseAuth)


        Dim response As New PacketWriter(OpCodes.SMSG_AUTH_RESPONSE)

        'If wrong Client Version, close all
        If Not clientVersion = 3368 Then 'HardCoded needed WoW Build
            Console.WriteLine("Account: " & Client.Character.clientAccount & " has attempted to log in with wrong Client build " & clientVersion)

            response.WriteUInt8(AuthResponseCodes.WRONG_CLIENT)
            Client.SendWorldClient(response)
            Exit Sub
        End If


        'if account is already logged in, disconnected it and procede with new login...
        'ToDo


        'Check Login Data
        Try
            Dim realmDB As New SQLiteBase("realmDB")
            Dim result As DataTable = realmDB.Select("SELECT password FROM accounts WHERE username = '" & Client.Character.clientAccount & "'")
            realmDB.DisposeDatabaseConnection()

            Dim Count As Integer = result.Rows.Count 'check, it should be nothing other than 0 or 1

            If Count = 0 Then 'Count = 0 mean no Username found
                If Config.AutoCreate Then 'if AutoCreate = true, then create account and login
                    Dim realmDB2 As New SQLiteBase("realmDB")
                    If realmDB2.Execute("INSERT INTO accounts (username, password) VALUES (" & _
                                                  "'{0}', '{1}')", Client.Character.clientAccount, clientPassword) Then
                        Console.WriteLine("[{0}] Account " & Client.Character.clientAccount & " successfully created!", Format(TimeOfDay, "HH:mm:ss"))
                        Console.WriteLine("Account: " & Client.Character.clientAccount & " logged in.")
                        response.WriteUInt8(AuthResponseCodes.AUTH_OK)
                    Else
                        Console.WriteLine("[{0}] Account Creation FAILED!", Format(TimeOfDay, "HH:mm:ss"))
                        response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
                    End If
                    realmDB2.DisposeDatabaseConnection()
                End If 'If Config.AutoCreate

            Else 'if Count = 1 then check for account password
                If result.Rows(0).ItemArray(0).ToString = clientPassword Then 'if Saved Password = Client Password
                    Console.WriteLine("Account: " & Client.Character.clientAccount & " logged in.")
                    response.WriteUInt8(AuthResponseCodes.AUTH_OK)
                Else
                    Console.WriteLine("Account: " & Client.Character.clientAccount & " tried to login with wrong password!")
                    response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
                End If 'If result.Rows ...

            End If 'If Count = 0 ...

        Catch ex As Exception
            Console.WriteLine("[{0}] Account Creation FAILED!" & Environment.NewLine & "{1}", Format(TimeOfDay, "HH:mm:ss"), ex.ToString)
            response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
        End Try

        'Send result
        Client.SendWorldClient(response)
    End Sub


    Public Sub On_CMSG_PING(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim response As New PacketWriter(OpCodes.SMSG_PONG)
        response.WriteUInt32(packet.ReadUInt32)
        Client.SendWorldClient(response)
    End Sub


End Module
