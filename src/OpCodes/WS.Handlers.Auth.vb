Imports System.Data
Imports System.Data.SQLite

Public Module WS_Handlers_Auth


    Public Sub On_CMSG_PLAYER_LOGIN(ByRef packet As PacketReader, ByRef Client As WorldServerClass)

        Console.WriteLine("[{0}] << CMSG_PLAYER_LOGIN", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)
        Console.WriteLine("ToDo: Send smsg_update_object")

        Dim PlayerGUID As ULong = 0
        PlayerGUID = packet.ReadUInt64() 'uint64 GUID


        'Temp: Send World Server down
        Dim down As New PacketWriter(OpCodes.SMSG_CHARACTER_LOGIN_FAILED)
        down.WriteUInt8(AuthLoginCodes.CHAR_LOGIN_NO_WORLD)
        Client.SendWorldClient(down)
        Console.WriteLine("[{0}] Unable to login: WORLD SERVER DOWN", Format(TimeOfDay, "hh:mm:ss"), Client.WSIP, Client.WSPort)
        Exit Sub
        '----------------------------


        Dim CharacterObject As New WS_Handlers_Char.CharacterObject(PlayerGUID, Client)

        Dim response As New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)
        Client.SendWorldClient(CharacterObject.Build_SMSG_UPDATE_OBJECT(packet, response))


        Console.WriteLine("Successfully sent: SMSG_UPDATE_OBJECT")
        Console.WriteLine("[{0}] Player GUID: {1} try to enter world ...", Format(TimeOfDay, "hh:mm:ss"), PlayerGUID, Client.WSIP, Client.WSPort)



        'ToDo:
        'Load Char Data
        'Send SMSG_UPDATE_OBJECT(BuildClientA9(Index, aSession(Index).GetGUID))

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
        Dim name As String = packet.ReadString()
        Dim race As Byte = packet.ReadByte()
        Dim pClass As Byte = packet.ReadByte()
        Dim gender As Byte = packet.ReadByte()
        Dim skin As Byte = packet.ReadByte()
        Dim face As Byte = packet.ReadByte()
        Dim hairStyle As Byte = packet.ReadByte()
        Dim hairColor As Byte = packet.ReadByte()
        Dim facialHair As Byte = packet.ReadByte()


        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name from characters")

        Dim Count As Integer = result.Rows.Count
        For i As Integer = 0 To Count - 1
            If result.Rows(i).ItemArray(0).ToString() = name Then 'if name already in Database:
                CharDB.DisposeDatabaseConnection() 'close Database
                response.WriteUInt8(CharCreateResponseCodes.NAME_ALREADY_TAKEN)
                Return response 'send response to client
                Exit Function 'exit this function
            End If
        Next

        'if name is not in Database:
        CharDB.Execute("INSERT INTO characters (name, account_id, race, class, gender, skin, face, hairstyle, haircolor, facialhair) VALUES (" & _
                       "'{0}', 1, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})", name, race, pClass, gender, skin, face, hairStyle, hairColor, facialHair)

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
            Console.WriteLine("Read Information of Character GUID: " & i.ToString)

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
                                                "haircolor, facialhair, level, zone, map, x, y, z, guildId, petdisplayId, " & _
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

        Catch ex As Exception
            realmDB.DisposeDatabaseConnection()
            response.WriteUInt8(AuthResponseCodes.AUTH_FAILED)
            Client.SendWorldClient(response)
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
