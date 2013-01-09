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

Public Enum OpCodes As UInteger
	'OpCodes: Server to Client
	SMSG_CHAR_CREATE = &H3A            ' 58
	SMSG_CHAR_ENUM = &H3B              ' 59
	SMSG_CHAR_DELETE = &H3C            ' 60
	SMSG_CHARACTER_LOGIN_FAILED = &H41 ' 65
	SMSG_LOGOUT_RESPONSE = &H4C        ' 76
	SMSG_LOGOUT_COMPLETE = &H4D        ' 77
	SMSG_NAME_QUERY_RESPONSE = &H51    ' 81
	SMSG_MESSAGECHAT = &H96            '150
	SMSG_UPDATE_OBJECT = &HA9          '169
	SMSG_AUTH_RESPONSE = &H1DF         '279
	SMSG_QUERY_TIME_RESPONSE = &H1C0   '448
	SMSG_PONG = &H1CE                  '462
	SMSG_AUTH_CHALLENGE = &H1DD        '477
	SMSG_INITIAL_SPELLS = &H11D        '
	SMSG_LEARNED_SPELL = &H11E         '
	SMSG_TRADE_STATUS = &H113          '
	SMSG_NEW_WORLD = &H3E              '
	SMSG_MOVE_WORLDPORT_ACK = &HC7     '
	
	'OpCodes: Client to Server
	CMSG_CHAR_CREATE = &H36            ' 54
	CMSG_CHAR_ENUM = &H37              ' 55
	CMSG_CHAR_DELETE = &H38            ' 56
	CMSG_PLAYER_LOGIN = &H3D           ' 61
	CMSG_LOGOUT_REQUEST = &H4B         ' 75
	CMSG_NAME_QUERY = &H50             ' 80
	CMSG_MESSAGECHAT = &H95            '149
	CMSG_QUERY_TIME = &H1BF            '447
	CMSG_PING = &H1CD                  '461
	CMSG_AUTH_SESSION = &H1DE          '478
	CMSG_ZONEUPDATE = &H1E5            '485
	CMSG_CANCEL_TRADE = &H10F          '271
	CMSG_REPORT_SCREENSHOT = &H1D8     '472
	CMSG_WORLD_TELEPORT = &H8          '
	
	'OpCodes: both ways
	MSG_MOVE_WORLDPORT_ACK = &HD9      '
	MSG_MOVE_START_FORWARD = &HB5      '
	MSG_MOVE_START_BACKWARD = &HB6     '
	MSG_MOVE_STOP = &HB7               '
	MSG_MOVE_START_STRAFE_LEFT = &HB8  '
	MSG_MOVE_START_STRAFE_RIGHT = &HB9 '
	MSG_MOVE_STOP_STRAFE = &HBA        '
	MSG_MOVE_JUMP = &HBB               '
	MSG_MOVE_START_TURN_LEFT = &HBC    '
	MSG_MOVE_START_TURN_RIGHT = &HBD   '
	MSG_MOVE_STOP_TURN = &HBE          '
	MSG_MOVE_START_PITCH_UP = &HBF     '
	MSG_MOVE_START_PITCH_DOWN = &HC0   '
	MSG_MOVE_STOP_PITCH = &HC1         '
	MSG_MOVE_SET_RUN_MODE = &HC2       '
	MSG_MOVE_SET_WALK_MODE = &HC3      '
	MSG_MOVE_START_SWIM = &HCB         '
	MSG_MOVE_STOP_SWIM = &HCC          '
	MSG_MOVE_SET_FACING = &HD7         '
	MSG_MOVE_SET_PITCH = &HD8          '
	MSG_MOVE_ROOT = &HE7               '
	MSG_MOVE_UNROOT = &HE8             '
	MSG_MOVE_HEARTBEAT = &HE9          '
End Enum


Enum CharCreateResponseCodes As Byte
	SUCCESS = &H28 '40 (Success)
	NAME_ALREADY_TAKEN = &H2B '43 (Name already taken)
End Enum


Enum AuthLoginCodes As Byte
	CHAR_LOGIN_FAILED = 0               'Login failed
	CHAR_LOGIN_NO_WORLD = 1             'World server is down
	CHAR_LOGIN_DUPLICATE_CHARACTER = 2  'A character with that name already exists
	CHAR_LOGIN_NO_INSTANCES = 3         'No instance servers are available
	CHAR_LOGIN_DISABLED = 4             'Login for that race and/or class is currently disabled
	CHAR_LOGIN_NO_CHARACTER = 5         'Character not found
	CHAR_LOGIN_LOCKED_FOR_TRANSFER = 6  '?
	CHAR_LOGIN_LOCKED_BY_BILLING = 7    '?
End Enum


Enum AuthResponseCodes As Byte
	WRONG_CLIENT = &H6 '6
	AUTH = &HB '11
	AUTH_OK = &HC '12
	AUTH_FAILED = &HD '13
	QUEUE = &H1B  '27
	
	'[BYTE] - Auth response
	'6   Wrong client version
	'11  Authenticating
	'12  Authentication successful
	'13  Authentication failed
	'24  Server shutting down
	'27  Position in queue
	'    [INT] - Position
End Enum


Enum TradeStatus As UInteger
	TRADE_STATUS_PLAYER_BUSY = &H0
	TRADE_STATUS_PROPOSED = &H1
	TRADE_STATUS_INITIATED = &H2
	TRADE_STATUS_CANCELLED = &H3
	TRADE_STATUS_ACCEPTED = &H4
	TRADE_STATUS_ALREADY_TRADING = &H5
	TRADE_STATUS_PLAYER_NOT_FOUND = &H6
	TRADE_STATUS_STATE_CHANGED = &H7
	TRADE_STATUS_COMPLETE = &H8
	TRADE_STATUS_UNACCEPTED = &H9
	TRADE_STATUS_TOO_FAR_AWAY = &HA
	TRADE_STATUS_WRONG_FACTION = &HB
	TRADE_STATUS_FAILED = &HC
	TRADE_STATUS_DEAD = &HD
	TRADE_STATUS_PETITION = &HE
	TRADE_STATUS_PLAYER_IGNORED = &HF
End Enum