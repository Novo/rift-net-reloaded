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


Public Module WS_Handlers_Chat
	
	Public Sub SendChatMessage(ByRef Client As WorldServerClass, ByVal ChatText As String, Optional PlayerGUID As UInt64 = 0, Optional ByVal MessageType As Byte = 0, Optional Language As UInt32 = 0, Optional ByVal SendToSelf As Boolean = false)
		Dim response As New PacketWriter(OpCodes.SMSG_MESSAGECHAT)
		
		'If no PlayerGUID then MessageType = SystemMessage (9)
		If PlayerGUID = 0 Then MessageType = 9
		
		'If SendToSelf = true then only to myself, else to all player	
		If SendToSelf = True Then
			response.WriteUInt8(MessageType)  ' slashCmd, 9: SystemMessage
			response.WriteUInt32(Language)    ' Language: General
			response.WriteUInt64(PlayerGUID)  ' Guid: 0 - ToAll???
			response.WriteString(ChatText)
			response.WriteUInt8(0)            ' afkDND, 0: nothing
			
			Client.SendWorldClient(response)
			
		Else		
			response.WriteUInt8(MessageType)  ' slashCmd, 9: SystemMessage
			response.WriteUInt32(Language)    ' Language: General
			response.WriteUInt64(PlayerGUID)  ' Guid: 0 - ToAll???
			response.WriteString(ChatText)
			response.WriteUInt8(0)            ' afkDND, 0: nothing
			Client.SendWorldClient(response)
			
			
			Dim tempSessions = New Dictionary(Of ULong, WorldServerClass)(Globals.WorldMgr.Sessions)
			tempSessions.Remove(Client.Character.GUID)
			
			If tempSessions IsNot Nothing Then
				
				For Each s As KeyValuePair(Of ULong, WorldServerClass) In tempSessions
					If Client.character.MapID <> s.Value.Character.MapID Then
						Continue For
					End If
					
					s.value.SendWorldClient(response)
				Next
				
				'			For Each s As KeyValuePair(Of ULong, WorldServerClass) In tempSessions
				'				Dim pChar As CharacterObject = s.Value.Character
				'				
				'				If pChar.MapID <> Client.character.MapID Then
				'					Continue For
				'				End If
				'				
				'				Client.SendWorldClient(response)
				'			Next
				
			End If
			
		End If
	End Sub
	
	
	Public Sub On_CMSG_MESSAGECHAT(ByRef packet As PacketReader, ByRef Client As WorldServerClass)
		Dim MessageType As Byte = CByte(packet.ReadInt32)
		Dim Language As UInt32 = packet.ReadUInt32
        Dim ChatText As String = packet.ReadString

        SendChatMessage(Client, ChatText, Client.Character.GUID, MessageType, Language)
    End Sub
	
	
	
End Module