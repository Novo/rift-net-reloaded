'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2013 noVo aka. takeoYasha www.easy-emu.de

'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports System.Data


Public Class WS_Handlers_World
    Inherits Globals


    Public Shared Sub Send_Update_Object(ByRef response As PacketWriter, ByRef Client As WorldServerClass, ByVal flags As UInt32, Optional Character As CharacterObject = Nothing)
        If IsNothing(Character) Then
            Character = Client.Character
        End If

        response.WriteUInt64(0) ' TransportGuid
        response.WriteSingle(0) ' TransportX
        response.WriteSingle(0) ' TransportY
        response.WriteSingle(0) ' TransportZ
        response.WriteSingle(0) ' TransportW (TransportO)

        response.WriteSingle(Character.PositionX)   ' x
        response.WriteSingle(Character.PositionY)   ' y
        response.WriteSingle(Character.PositionZ)   ' z
        response.WriteSingle(Character.Orientation) ' o
        response.WriteSingle(0)         ' Pitch
        response.WriteUInt32(&H8000000) ' MovementFlagMask
        response.WriteUInt32(0)         ' FallTime
        response.WriteSingle(2.5)       ' WalkSpeed
        response.WriteSingle(7.0)       ' RunSpeed
        response.WriteSingle(4.7222)    ' SwimSpeed
        response.WriteSingle(3.14)      ' TurnSpeed
        response.WriteUInt32(flags)     ' Flags, 1 - SelfUpdate
        response.WriteUInt32(1)         ' AttackCycle
        response.WriteUInt32(0)         ' TimerId
        response.WriteUInt64(0)         ' VictimGuid

        ' FillInPartialObjectData
        response.WriteUInt8(&H14)

        ' UpdateMaskBlocks, 20
        For i As Integer = 0 To 19
            response.WriteUInt32(&HFFFFFFFFUI)
        Next

        ' ObjectFields
        response.WriteUInt64(Character.GUID)
        response.WriteUInt32(&H19)
        ' UpdateType, 0x19 - Player (Player + Unit + Object)
        response.WriteUInt32(0)
        response.WriteSingle(1)
        response.WriteUInt32(0)

        ' UnitFields
        For i As Integer = 0 To 15
            response.WriteUInt32(0)
        Next

        response.WriteUInt32(Character.Health)
        response.WriteUInt32(Character.Mana)
        response.WriteUInt32(Character.Rage)
        response.WriteUInt32(Character.Focus)
        response.WriteUInt32(Character.Energy)

        ' Max values...
        response.WriteUInt32(Character.Health)
        response.WriteUInt32(Character.Mana)
        response.WriteUInt32(Character.Rage)
        response.WriteUInt32(Character.Focus)
        response.WriteUInt32(Character.Energy)
        response.WriteUInt32(Character.Level)
        response.WriteUInt32(5)
        response.WriteUInt8(Character.Race)
        response.WriteUInt8(Character.Classe)
        response.WriteUInt8(Character.Gender)
        response.WriteUInt8(1)

        response.WriteUInt32(Character.Strength)
        response.WriteUInt32(Character.Agility)
        response.WriteUInt32(Character.Stamina)
        response.WriteUInt32(Character.Intellect)
        response.WriteUInt32(Character.Spirit)

        ' BastStats, copy ...
        response.WriteUInt32(Character.Strength)
        response.WriteUInt32(Character.Agility)
        response.WriteUInt32(Character.Stamina)
        response.WriteUInt32(Character.Intellect)
        response.WriteUInt32(Character.Spirit)

        For i As Integer = 0 To 9
            response.WriteUInt32(0)
        Next

        ' Money
        response.WriteUInt32(Character.Money)

        For i As Integer = 0 To 55
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 38
            response.WriteUInt32(0)
        Next

        ' DisplayId
        Select Case Character.Race
            Case GlobalConstants.Races.RACE_HUMAN
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H31, &H32)))
                Exit Select
            Case GlobalConstants.Races.RACE_ORC
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H33, &H34)))
                Exit Select
            Case GlobalConstants.Races.RACE_DWARF
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H35, &H36)))
                Exit Select
            Case GlobalConstants.Races.RACE_NIGHT_ELF
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H37, &H38)))
                Exit Select
            Case GlobalConstants.Races.RACE_UNDEAD
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H39, &H40))) 'bug: female undead cube model
                Exit Select
            Case GlobalConstants.Races.RACE_TAUREN
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H3B, &H3C)))
                Exit Select
            Case GlobalConstants.Races.RACE_GNOME
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H61B, &H61C)))
                Exit Select
            Case GlobalConstants.Races.RACE_TROLL
                response.WriteUInt32(CUInt(If(Character.Gender = 0, &H5C6, &H5C7)))
                Exit Select
        End Select

        For i As Integer = 0 To 31
            response.WriteUInt32(0)
        Next

        ' PlayerFields
        For i As Integer = 0 To 45
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 31
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 47
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 11
            response.WriteUInt32(0)
        Next

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        ' InventarSlots
        response.WriteUInt32(14)

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        ' PLAYER_BYTES (Skin, Face, HairStyle, HairColor)
        response.WriteUInt8(Character.Skin)
        response.WriteUInt8(Character.Face)
        response.WriteUInt8(Character.HairStyle)
        response.WriteUInt8(Character.HairColor)

        ' XP
        response.WriteUInt32(0)

        ' NextLevel
        response.WriteUInt32(1)

        ' SkillInfo
        For i As Integer = 0 To 191
            response.WriteUInt32(0)
        Next

        ' PLAYER_BYTES_2 (FacialHair, Unknown, BankBagSlotCount, RestState)
        response.WriteUInt8(Character.FacialHair)
        response.WriteUInt8(0)
        response.WriteUInt8(0)
        response.WriteUInt8(1)

        ' QuestInfo
        For i As Integer = 0 To 95
            response.WriteUInt32(0)
        Next

        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)

        ' BaseMana
        response.WriteUInt32(20)
        response.WriteUInt32(0)

        ' unknown
        response.WriteUInt8(0)
        response.WriteUInt8(0)
        response.WriteUInt8(0)
    End Sub


    Public Shared Sub Build_SMSG_UPDATE_OBJECT(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
        Dim PlayerGUID As UInt64 = packet.ReadUInt64()

        'Create a new CharacterObject and get Char by GUID from Database ...
        Client.Character = New CharacterObject(PlayerGUID)
        WorldMgr.AddSession(PlayerGUID, Client)


        Dim response As New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)

        response.WriteUInt32(1) ' ObjectCount
        response.WriteUInt8(2)  ' UpdateType, UPDATE_FULL/CreateObject) + PartialUpdateFromFullUpdate
        response.WriteUInt64(Client.Character.GUID) ' ObjectGuid
        response.WriteUInt8(4)  ' ObjectType, 4 = Player

        Send_Update_Object(response, Client, 1)

        Client.SendWorldClient(response)


        Dim tempSessions = New Dictionary(Of ULong, WorldServerClass)(WorldMgr.Sessions)
        tempSessions.Remove(Client.Character.GUID)

        If tempSessions IsNot Nothing Then

            For Each s As KeyValuePair(Of ULong, WorldServerClass) In tempSessions
                If Client.Character.MapID <> s.Value.Character.MapID Then
                    Continue For
                End If

                response = New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)

                response.WriteUInt32(1)
                response.WriteUInt8(2)
                response.WriteUInt64(Client.Character.GUID)
                response.WriteUInt8(4)

                Send_Update_Object(response, Client, 0)

                s.Value.SendWorldClient(response)
            Next

            For Each s As KeyValuePair(Of ULong, WorldServerClass) In tempSessions
                Dim pChar As CharacterObject = s.Value.Character

                If pChar.MapID <> Client.Character.MapID Then
                    Continue For
                End If

                response = New PacketWriter(OpCodes.SMSG_UPDATE_OBJECT)

                response.WriteUInt32(1)
                response.WriteUInt8(2)
                response.WriteUInt64(pChar.GUID)
                response.WriteUInt8(4)

                Send_Update_Object(response, Client, 0, pChar)

                Client.SendWorldClient(response)
            Next
        End If
    End Sub



End Class