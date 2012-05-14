Imports System.Data
Imports System.Data.SQLite

Public Module WS_Handlers_Auth


    Public Sub On_HandleMovementStatus(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

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

        WS.Character.PositionX = X
        WS.Character.PositionY = Y
        WS.Character.PositionZ = Z
        WS.Character.Facing = O


        'ToDo: Save in DB every x seconds
    End Sub


    Public Sub On_CMSG_MESSAGECHAT(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] << CMSG_MESSAGECHAT", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim response As New PacketWriter(OpCodes.SMSG_MESSAGECHAT)
        response.WriteUInt8(9)     ' slashCmd, 9: SystemMessage
        response.WriteUInt32(0)    ' Language: General
        response.WriteUInt64(0)    ' Guid: 0 - ToAll???
        response.WriteString("Hallo Novo :)")
        response.WriteUInt8(0)     'afkDND, 0: nothing

        Client.SendWorldClient(response)

        Console.WriteLine("Successfully sent: SMSG_MESSAGECHAT")
    End Sub


    Public Sub On_CMSG_LOGOUT_REQUEST(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] << CMSG_LOGOUT_REQUEST", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim response As New PacketWriter(OpCodes.SMSG_LOGOUT_COMPLETE)
        Client.SendWorldClient(response)



        'Save Character Position and such stuff

        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name from characters")


        Dim Count As Integer = result.Rows.Count
        For i As Integer = 0 To Count - 1
            If result.Rows(i).ItemArray(0).ToString() = WS.Character.Name Then 'if name already in Database:

                'WS.Character.GUID

                'if name is in Database:                  '0     '1   '2 '3 '4    '5
                CharDB.Execute("UPDATE characters SET (zoneID, mapID, x, y, z, facing) VALUES (" & _
                               "        {0},                {1},                           {2},                                                                             {3},                                                                              {4},                                                                     {5} WHERE GUID = " & WS.Character.GUID & " )", _
                               WS.Character.ZoneID, WS.Character.MapID, WS.Character.PositionX.ToString("F", Globalization.CultureInfo.InvariantCulture), WS.Character.PositionY.ToString("F", Globalization.CultureInfo.InvariantCulture), WS.Character.PositionZ.ToString("F", Globalization.CultureInfo.InvariantCulture), WS.Character.Facing.ToString("F", Globalization.CultureInfo.InvariantCulture))
                Exit For 'exit this function
            End If
        Next

        CharDB.DisposeDatabaseConnection()

        Console.WriteLine("Character saved!")


        Console.WriteLine("Successfully sent: SMSG_LOGOUT_COMPLETE")
    End Sub


    Public Sub On_CMSG_NAME_QUERY(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] << CMSG_NAME_QUERY", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)


        Dim PlayerGUID As ULong = 0
        PlayerGUID = packet.ReadUInt64() 'uint64 GUID

        'Dim Character As New WS_Handlers_Char.CharacterObject()

        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name, race, class, gender FROM characters WHERE GUID = " & PlayerGUID.ToString)
        CharDB.DisposeDatabaseConnection()


        WS.Character.Name = DirectCast(result.Rows(0).ItemArray(0), String)
        WS.Character.Race = CType(result.Rows(0).ItemArray(1), GlobalConstants.Races)
        WS.Character.Classe = CType(result.Rows(0).ItemArray(2), GlobalConstants.Classes)
        WS.Character.Gender = CType(result.Rows(0).ItemArray(3), GlobalConstants.Genders)


        Dim response As New PacketWriter(OpCodes.SMSG_NAME_QUERY_RESPONSE)

        response.WriteUInt64(PlayerGUID) 'player guid
        response.WriteString(WS.Character.Name) 'player name
        response.WriteUInt32(WS.Character.Race) 'race
        response.WriteUInt32(WS.Character.Gender) 'gender
        response.WriteUInt32(WS.Character.Classe) 'class
        response.WriteUInt8(0) '// tell the server this name is not declined...

        Client.SendWorldClient(response)


        Console.WriteLine("Successfully sent: SMSG_NAME_QUERY_RESPONSE")
    End Sub


    Public Sub On_CMSG_PLAYER_LOGIN(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_PLAYER_LOGIN", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)


        'Temp: Send World Server down
        'Dim down As New PacketWriter(OpCodes.SMSG_CHARACTER_LOGIN_FAILED)
        'down.WriteUInt8(AuthLoginCodes.CHAR_LOGIN_DISABLED)
        'Client.SendWorldClient(down)
        'Console.WriteLine("[{0}] Unable to login: WORLD SERVER DOWN", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)
        'Exit Sub
        '----------------------------

        Dim NewWorld As New WS_Handlers_World

        Dim response As New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)
        Client.SendWorldClient(NewWorld.Build_SMSG_UPDATE_OBJECT(packet, response, Client))


        'frmMain.World(Index).SendData(SMSG_MESSAGECHAT(Hex2ASCII("09 00 00 00 00 00 00 00 00 00 00 00 00") & "=====================================" & NT & Chr(&H0)))
        'frmMain.World(Index).SendData(SMSG_MESSAGECHAT(Hex2ASCII("09 00 00 00 00 00 00 00 00 00 00 00 00") & "Welcome to " & ServerName & NT & Chr(&H0)))
        'frmMain.World(Index).SendData(SMSG_MESSAGECHAT(Hex2ASCII("09 00 00 00 00 00 00 00 00 00 00 00 00") & "Running Rift 2 v." & Version & NT & Chr(&H0)))
        'frmMain.World(Index).SendData(SMSG_MESSAGECHAT(Hex2ASCII("09 00 00 00 00 00 00 00 00 00 00 00 00") & "Server-wide save every " & frmMain.SaveEvery_Minutes & " minutes." & NT & Chr(&H0)))
        'frmMain.World(Index).SendData(SMSG_MESSAGECHAT(Hex2ASCII("09 00 00 00 00 00 00 00 00 00 00 00 00") & "!help for server command help." & NT & Chr(&H0)))
        'frmMain.World(Index).SendData(SMSG_MESSAGECHAT(Hex2ASCII("09 00 00 00 00 00 00 00 00 00 00 00 00") & "=====================================" & NT & Chr(&H0)))
        'Console.ForegroundColor = System.ConsoleColor.White
        'Console.WriteLine(CType(System.Reflection.[Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyTitleAttribute), False)(0), System.Reflection.AssemblyTitleAttribute).Title)
        'Console.Write("Version {0}", System.Reflection.[Assembly].GetExecutingAssembly().GetName().Version)
        'Console.WriteLine()


        'DAFUQ? Need Chat Handler ...
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
        chat_player.WriteString("Player: " & WS.Character.Name & " logged in.")
        chat_player.WriteUInt8(0)     'afkDND, 0: nothing
        Client.SendWorldClient(chat_player)


        Console.WriteLine("Successfully sent: SMSG_UPDATE_OBJECT")
        'Console.WriteLine("[{0}] Player GUID: {1} try to enter world ...", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)
    End Sub


    Public Sub On_CMSG_CHAR_DELETE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] << CMSG_CHAR_DELETE", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim PlayerGUID As ULong = 0
        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_DELETE)

        Try
            PlayerGUID = packet.ReadUInt64() 'uint64 GUID

            Dim success As Boolean = False
            Dim CharDB As New SQLiteBase("characterDB")
            Dim result As DataTable = CharDB.Select("SELECT name from characters")

            success = CharDB.Execute("DELETE from characters WHERE guid = " & PlayerGUID.ToString)


            If success Then
                Console.WriteLine("[{0}] Player GUID: {1} deleted...", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)
            Else
                Console.WriteLine("[{0}] Player GUID: {1} deletion FAILED!", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)
            End If


            Client.SendWorldClient(response)

            CharDB.DisposeDatabaseConnection()

        Catch ex As Exception
            Console.WriteLine("[{0}] Player GUID: {1} deletion FAILED!", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)
        End Try

    End Sub


    Public Function HandleCharCreate(ByRef packet As PacketReader, ByRef response As PacketWriter) As PacketWriter

        'Dim Character As New WS_Handlers_Char.CharacterObject()
        Dim AccountID As Integer = 1

        WS.Character.Name = packet.ReadString()
        WS.Character.Race = CType(packet.ReadByte(), GlobalConstants.Races)
        WS.Character.Classe = CType(packet.ReadByte(), GlobalConstants.Classes)
        WS.Character.Gender = CType(packet.ReadByte(), GlobalConstants.Genders)
        WS.Character.Skin = packet.ReadByte()
        WS.Character.Face = packet.ReadByte()
        WS.Character.HairStyle = packet.ReadByte()
        WS.Character.HairColor = packet.ReadByte()
        WS.Character.FacialHair = packet.ReadByte()



        'ToDo: find the correct start coordinates
        WS.Character.ZoneID = 1
        WS.Character.MapID = 0
        WS.Character.PositionX = -6240.319824
        WS.Character.PositionY = 331.03299
        WS.Character.PositionZ = 382.757996
        WS.Character.Facing = 1

        ''Save Start Area:
        'Select Case Character.Race
        '    Case GlobalConstants.Races.RACE_HUMAN
        '        Character.ZoneID = 12
        '        Character.MapID = 0
        '        Character.PositionX = -8942.044
        '        Character.PositionY = -130.8001
        '        Character.PositionZ = 84.83379
        '        Character.Facing = 0.5806077
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_ORC
        '        Character.ZoneID = 14
        '        Character.MapID = 1
        '        Character.PositionX = -618.518005
        '        Character.PositionY = -4251.669922
        '        Character.PositionZ = 38.717999
        '        Character.Facing = 1
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_DWARF
        '        Character.ZoneID = 1
        '        Character.MapID = 0
        '        Character.PositionX = -6240.319824
        '        Character.PositionY = 331.03299
        '        Character.PositionZ = 382.757996
        '        Character.Facing = 1
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_NIGHT_ELF
        '        Character.ZoneID = 141
        '        Character.MapID = 1
        '        Character.PositionX = -10311.943359
        '        Character.PositionY = 832.356689
        '        Character.PositionZ = 1326.395752
        '        Character.Facing = 0.210676
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_UNDEAD
        '        Character.ZoneID = 14
        '        Character.MapID = 1
        '        Character.PositionX = -618.518005
        '        Character.PositionY = -4251.669922
        '        Character.PositionZ = 38.717999
        '        Character.Facing = 1
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_TAUREN
        '        Character.ZoneID = 215
        '        Character.MapID = 1
        '        Character.PositionX = -2917.580078
        '        Character.PositionY = -257.980011
        '        Character.PositionZ = 52.996799
        '        Character.Facing = 1
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_GNOME
        '        Character.ZoneID = 1
        '        Character.MapID = 0
        '        Character.PositionX = -6240.319824
        '        Character.PositionY = 331.03299
        '        Character.PositionZ = 382.757996
        '        Character.Facing = 1
        '        Exit Select

        '    Case GlobalConstants.Races.RACE_TROLL
        '        Character.ZoneID = 14
        '        Character.MapID = 1
        '        Character.PositionX = -618.518005
        '        Character.PositionY = -4251.669922
        '        Character.PositionZ = 38.717999
        '        Character.Facing = 1
        '        Exit Select
        'End Select


        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name from characters")

        Dim Count As Integer = result.Rows.Count
        For i As Integer = 0 To Count - 1
            If result.Rows(i).ItemArray(0).ToString() = WS.Character.Name Then 'if name already in Database:
                CharDB.DisposeDatabaseConnection() 'close Database
                response.WriteUInt8(CharCreateResponseCodes.NAME_ALREADY_TAKEN)
                Return response 'send response to client
                Exit Function 'exit this function
            End If
        Next



        'if name is not in Database:                '0      '1    '2     '3     '4      '5    '6      '7         '8         '9        '10    '11  '12'13'14   '15
        CharDB.Execute("INSERT INTO characters (account_id, name, race, class, gender, skin, face, hairstyle, haircolor, facialhair, zoneID, mapID, x, y, z, facing) VALUES (" & _
                       "  {0},        '{1}',                           {2},                            {3},                              {4},               {5},            {6},            {7},                 {8},                 {9},                 {10},             {11},            {12},                {13},                {14},                {15})", _
                       AccountID, WS.Character.Name, Convert.ToByte(WS.Character.Race), Convert.ToByte(WS.Character.Classe), Convert.ToByte(WS.Character.Gender), WS.Character.Skin, WS.Character.Face, WS.Character.HairStyle, WS.Character.HairColor, WS.Character.FacialHair, WS.Character.ZoneID, WS.Character.MapID, WS.Character.PositionX.ToString("F", Globalization.CultureInfo.InvariantCulture), WS.Character.PositionY.ToString("F", Globalization.CultureInfo.InvariantCulture), WS.Character.PositionZ.ToString("F", Globalization.CultureInfo.InvariantCulture), WS.Character.Facing.ToString("F", Globalization.CultureInfo.InvariantCulture))


        CharDB.DisposeDatabaseConnection()

        response.WriteUInt8(CharCreateResponseCodes.SUCCESS)
        Return response
    End Function


    Public Sub On_CMSG_CHAR_CREATE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_CHAR_CREATE", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_CREATE)
        Client.SendWorldClient(HandleCharCreate(packet, response))

        Console.WriteLine("Successfully sent: SMSG_CHAR_CREATE")
    End Sub


    Public Function HandleCharEnum(ByVal DBresult As DataTable, ByVal bNumChars As Byte, ByRef response As PacketWriter) As PacketWriter

        For i As Integer = 0 To bNumChars - 1
            Console.WriteLine("Read Information of Character: " & (i).ToString)

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

        Console.WriteLine("[{0}] << CMSG_CHAR_ENUM", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim bNumChars As Byte = 0

        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT guid, name, race, class, gender, skin, face, hairstyle, " & _
                                                "haircolor, facialhair, level, zoneID, mapID, x, y, z, guildId, petdisplayId, " & _
                                                "petlevel, petfamily FROM characters WHERE account_id = 1")

        bNumChars = CByte(result.Rows.Count)

        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_ENUM)
        response.WriteUInt8(bNumChars) 'Number of Characters
        Client.SendWorldClient(HandleCharEnum(result, bNumChars, response))

        CharDB.DisposeDatabaseConnection()

        Console.WriteLine("Successfully sent: SMSG_CHAR_ENUM")
    End Sub


    Public Sub On_CMSG_AUTH_SESSION(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_AUTH_SESSION", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim crypt As New Crypt
        Dim clientVersion As Integer = packet.ReadInt32
        Dim clientSesionID As Integer = packet.ReadInt32
        Dim clientAccount As String = packet.ReadAccountName
        Dim clientPassword As String = packet.ReadString


        'Start Sending Auth ...
        ' Dim responseAuth As New PacketWriter(OpCodes.SMSG_AUTH_RESPONSE)
        ' responseAuth.WriteUInt8(AuthResponseCodes.AUTH)
        ' Client.SendWorldClient(responseAuth)


        Dim response As New PacketWriter(OpCodes.SMSG_AUTH_RESPONSE)

        'If wrong Client Version, close all
        If Not clientVersion = 3368 Then 'HardCoded needed WoW Build
            Console.WriteLine("Account: " & clientAccount & " has attempted to log in with wrong Client build " & clientVersion)

            response.WriteUInt8(AuthResponseCodes.WRONG_CLIENT)
            Client.SendWorldClient(response)
            Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
            Exit Sub
        End If


        'if account is already logged in, disconnected it and procede with new login...
        'ToDo


        'If wrong Password, close all
        Dim realmDB As New SQLiteBase("realmDB")

        Try
            Dim result As DataTable = realmDB.Select("SELECT password FROM accounts WHERE username = '" & clientAccount & "'")
            realmDB.DisposeDatabaseConnection()

            Dim Count As Integer = result.Rows.Count

            For i As Integer = 0 To Count - 1

                If result.Rows(i).ItemArray(0).ToString = clientPassword Then
                    response.WriteUInt8(AuthResponseCodes.AUTH_OK)
                    Client.SendWorldClient(response)

                    Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
                    Console.WriteLine("Account: " & clientAccount & " logged in.")
                    Exit Sub
                Else
                    If Config.AutoCreate Then


                        'Try
                        '    Dim crypt As New Crypt
                        '    Dim RealmDB As New SQLiteBase("realmDB")
                        '    Dim result As DataTable = RealmDB.Select("SELECT username from accounts")
                        '    Dim alreadyexist As Boolean = False
                        '    Dim success As Boolean = False

                        '    Dim Count As Integer = result.Rows.Count

                        '    For i As Integer = 0 To Count - 1
                        '        If result.Rows(i).ItemArray(0).ToString() = cmds(1) Then 'if username is already in Database:
                        '            alreadyexist = True
                        '            RealmDB.DisposeDatabaseConnection()
                        '            Console.WriteLine("[{0}] Account already exist!", Format(TimeOfDay, "hh:mm:ss"))
                        '            Exit For
                        '        End If
                        '    Next

                        '    If Not alreadyexist Then
                        '        success = RealmDB.Execute("INSERT INTO accounts (username, password) VALUES (" & _
                        '                       "'{0}', '{1}')", cmds(2).ToUpper, crypt.getMd5Hash(cmds(3)))
                        '        RealmDB.DisposeDatabaseConnection()

                        '        If success Then
                        '            Console.WriteLine("[{0}] Account " & cmds(2) & " successfully created!", Format(TimeOfDay, "hh:mm:ss"))
                        '        Else
                        '            Console.WriteLine("[{0}] Account Creation FAILED!", Format(TimeOfDay, "hh:mm:ss"))
                        '        End If


                        '    End If

                        'Catch ex As Exception
                        '    Console.WriteLine("[{0}] Account Creation FAILED!", Format(TimeOfDay, "hh:mm:ss"))
                        'End Try

                        response.WriteUInt8(AuthResponseCodes.AUTH_OK)
                        Client.SendWorldClient(response)

                        Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
                        Console.WriteLine("Account: " & clientAccount & " logged in.")
                        Exit Sub
                    Else
                        realmDB.DisposeDatabaseConnection()
                        response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
                        Client.SendWorldClient(response)

                        Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
                        Console.WriteLine("Account: " & clientAccount & " tried to login with wrong password.")
                    End If

                    Exit Sub
                End If

            Next

            realmDB.DisposeDatabaseConnection()
            response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
            Client.SendWorldClient(response)
            Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
            Console.WriteLine("Account: " & clientAccount & " does not exist in Database!")

        Catch ex As Exception
            realmDB.DisposeDatabaseConnection()
            response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
            Client.SendWorldClient(response)
            Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
            Console.WriteLine("Account: " & clientAccount & " failed to log in!")
        End Try

    End Sub


    Public Sub On_CMSG_PING(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_PING", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim response As New PacketWriter(OpCodes.SMSG_PONG)
        response.WriteUInt32(packet.ReadUInt32)
        Client.SendWorldClient(response)

        Console.WriteLine("Successfully sent: SMSG_PONG")
    End Sub


End Module
