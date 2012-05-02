Public Module WS_Handlers_Auth


    Public Sub On_CMSG_PLAYER_LOGIN(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_PLAYER_LOGIN", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)
        Console.WriteLine("ToDo: Send smsg_update_object")

        Dim PlayerGUID As ULong = 0
        PlayerGUID = packet.ReadUInt64() 'uint64 GUID

        Console.WriteLine("[{0}] Player GUID: {1} try to enter world ...", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)


        'Temp: Send World Server down
        Dim response As New PacketWriter(OpCodes.SMSG_CHARACTER_LOGIN_FAILED)
        response.WriteInt8(AuthLoginCodes.CHAR_LOGIN_NO_WORLD)
        Client.SendWorldClient(response)
        Console.WriteLine("[{0}] Unable to login: WORLD SERVER DOWN", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)
        'Temp: Send World Server down


        'Dim CharacterObject As New WS_Handlers_Char.CharacterObject(PlayerGUID, Client)

        'Dim test As New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)
        'test.WriteString(CharacterObject.Build_SMSG_UPDATE_OBJECT(PlayerGUID))
        'Client.SendWorldClient(test)

        'Console.WriteLine("Successfully sent: SMSG_UPDATE_OBJECT")



        'ToDo:
        'Load Char Data
        'SendData SMSG_UPDATE_OBJECT(BuildClientA9(Index, aSession(Index).GetGUID))

    End Sub


    Public Sub On_CMSG_CHAR_DELETE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Console.WriteLine("[{0}] << CMSG_CHAR_DELETE", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)
        Console.WriteLine("ToDo: Delete Character and send SMSG_CHAR_ENUM(BuildCharEnum(Index))")

        Dim PlayerGUID As ULong = 0
        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_DELETE)

        Try
            PlayerGUID = packet.ReadUInt64() 'uint64 GUID

            'Do Database Stuff (delete char and stuff with GUID)

            Console.WriteLine("[{0}] Player GUID: {1} deleted. ...", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)
        Catch ex As Exception
            Console.WriteLine("[{0}] Player GUID: {1} deletion FAILED!", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)
        End Try

        Client.SendWorldClient(response)
    
    End Sub


    Public Sub On_CMSG_CHAR_CREATE(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_CHAR_CREATE", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        'ToDo:
        'ask character Info and write to Database


        Dim response As New PacketWriter(OpCodes.SMSG_CHAR_CREATE)
        response.WriteUInt8(43) 'Name already taken (40 = Success)
        Client.SendWorldClient(response)

        Console.WriteLine("Successfully sent: SMSG_CHAR_CREATE")
    End Sub


    Public Function BuildCharEnum(ByRef writer As PacketWriter) As PacketWriter

        'DEBUG: Test Character
        'writer.WriteUInt8(bNumChars) 'Number of Characters

        writer.WriteUInt64(1) 'GUID

        writer.WriteString("NoVo") 'Name

        writer.WriteUInt8(1) 'Race
        writer.WriteUInt8(1) 'Class
        writer.WriteUInt8(0) 'Gender
        writer.WriteUInt8(1) 'Skin
        writer.WriteUInt8(1) 'Face
        writer.WriteUInt8(1) 'HairStyle
        writer.WriteUInt8(1) 'HairColor
        writer.WriteUInt8(1) 'FacialHair
        writer.WriteUInt8(1) 'Player Level

        writer.WriteUInt32(0) 'ZoneID
        writer.WriteUInt32(0) 'MapID

        writer.WriteUInt32(0) 'CurX
        writer.WriteUInt32(0) 'CurY
        writer.WriteUInt32(0) 'CurZ

        writer.WriteUInt32(0) 'GuildID
        writer.WriteUInt32(0) 'PetDisp
        writer.WriteUInt32(0) 'PetLevel
        writer.WriteUInt32(0) 'PetFamID

        For j As Integer = 1 To 20 'ToDo: items [uint32 - Display ID][byte - item type]
            writer.WriteUInt32(0)
            writer.WriteUInt8(0)
        Next


        'Next Character


        'DEBUG: Test Character
        'writer.WriteUInt8(bNumChars) 'Number of Characters

        writer.WriteUInt64(2) 'GUID

        writer.WriteString("Fabi") 'Name

        writer.WriteUInt8(1) 'Race
        writer.WriteUInt8(1) 'Class
        writer.WriteUInt8(0) 'Gender
        writer.WriteUInt8(1) 'Skin
        writer.WriteUInt8(1) 'Face
        writer.WriteUInt8(1) 'HairStyle
        writer.WriteUInt8(1) 'HairColor
        writer.WriteUInt8(1) 'FacialHair
        writer.WriteUInt8(1) 'Player Level

        writer.WriteUInt32(0) 'ZoneID
        writer.WriteUInt32(0) 'MapID

        writer.WriteUInt32(0) 'CurX
        writer.WriteUInt32(0) 'CurY
        writer.WriteUInt32(0) 'CurZ

        writer.WriteUInt32(0) 'GuildID
        writer.WriteUInt32(0) 'PetDisp
        writer.WriteUInt32(0) 'PetLevel
        writer.WriteUInt32(0) 'PetFamID

        For j As Integer = 1 To 20 'ToDo: items [uint32 - Display ID][byte - item type]
            writer.WriteUInt32(0)
            writer.WriteUInt8(0)
        Next


        'Next Character


        Return writer
    End Function


    Public Sub On_CMSG_CHAR_ENUM(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_CHAR_ENUM", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim bNumChars As Byte = 2
        'Get Number of Chars from Database

        If bNumChars > 0 Then
            Dim response As New PacketWriter(OpCodes.SMSG_CHAR_ENUM)
            response.WriteUInt8(bNumChars) 'Number of Characters
            Client.SendWorldClient(BuildCharEnum(response))

        Else
            'Send Empty Char List
            Dim response As New PacketWriter(OpCodes.SMSG_CHAR_ENUM)
            response.WriteUInt8(0)
            Client.SendWorldClient(response)
        End If

        Console.WriteLine("Successfully sent: SMSG_CHAR_ENUM")
    End Sub



    Public Sub On_CMSG_AUTH_SESSION(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_AUTH_SESSION", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim clientVersion As Integer = packet.ReadInt32
        Dim clientSesionID As Integer = packet.ReadInt32
        Dim clientAccount As String = packet.ReadAccountName

        Dim UserName As String = ""
        Dim Password As String = ""

        UserName = clientAccount
        Password = ""


        Dim response As New PacketWriter(OpCodes.SMSG_AUTH_RESPONSE)

        ' If wrong Client Version, close all
        If Not clientVersion = 3368 Then 'HardCoded needed WoW Build
            Console.WriteLine("User: " & UserName & " has attempted to log in with wrong Client build " & clientVersion)

            response.WriteInt8(AuthResponseCodes.WRONG_CLIENT) '6 - Wrong Client Version
            Client.SendWorldClient(response)
            Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")
            Exit Sub
        End If

        response.WriteInt8(AuthResponseCodes.AUTH_OK) '12 - Success
        Client.SendWorldClient(response)

        Console.WriteLine("Successfully sent: SMSG_AUTH_RESPONSE")

        Console.WriteLine("User: " & UserName & " logged in")



        'ToDo:
        'aSession(Index) = New Session
        'If aSession(Index).InitSession(strUsername) = False Then 'If User does NOT exist
        '    If AutoCreate = "true" Then 'If AutoCreate = true then create and Login, else Incorrect Password
        '        SaveConfig(aSession(Index).GetAccountFile, "Account", "GM", "0")
        '        SaveConfig(aSession(Index).GetAccountFile, "Account", "Password", strPassword)
        '        SaveConfig(aSession(Index).GetAccountFile, "Account", "NumChars", 0)
        '        SaveConfig(aSession(Index).GetAccountFile, "Account", "Chars", "")
        '        COut("Account '" & strUsername & "' created.")
        '        frmMain.World(Index).SendData(SMSG_AUTH_RESPONSE(34)) 'account created
        '    Else
        '        COut("Account '" & strUsername & "' does not exist.")
        '        frmMain.World(Index).SendData(SMSG_AUTH_RESPONSE(22)) 'Incorrect Password
        '        Exit Sub
        '    End If
        'End If

        'If GetConfig(aSession(Index).GetAccountFile, "Account", "Password") = strPassword Then 'if the MD5 Hash match

        '    COut("Account: " & strUsername & " has logged in.")
        '    frmMain.World(Index).SendData(SMSG_AUTH_RESPONSE(12)) 'Success

        '    Dim ConnSockets As Integer
        '    For ConnSockets = 1 To totalSockets
        '        'if account is already logged in, disconnected it and procede with new login...
        '        If ConnSockets = Index Then 'If ConnSockets = me then do NOT
        '            'don't do shit
        '        ElseIf aSession(ConnSockets).GetAccount = strUsername Then
        '            frmMain.World(ConnSockets).Close()
        '        End If
        '        DoEvents()
        '    Next ConnSockets

        'Else
        '    COut(strUsername & " has failed to log in.")
        '    frmMain.World(Index).SendData(SMSG_AUTH_RESPONSE(22)) 'Incorrect Password
        'End If
        'Exit Sub
    End Sub


    Public Sub On_CMSG_PING(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_PING", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)

        Dim response As New PacketWriter(OpCodes.SMSG_PONG)
        response.WriteUInt32(packet.ReadUInt32)
        Client.SendWorldClient(response)

        Console.WriteLine("Successfully sent: SMSG_PONG")

    End Sub

End Module
