<Flags()> Public Enum OpCodes As UShort
    'Realm Login
    SMSG_AUTH_CHALLENGE = &H1DD '477
    SMSG_AUTH_RESPONSE = &H1DF '279
    SMSG_CHAR_ENUM = &H3B '59
    SMSG_PONG = &H1CE '462
    SMSG_CHAR_CREATE = &H3A '58
    SMSG_CHAR_DELETE = &H3C '60
    SMSG_CHARACTER_LOGIN_FAILED = &H41 '65
    SMSG_UPDATE_OBJECT = &HA9 '169

    CMSG_PING = &H1CD '461
    CMSG_AUTH_SESSION = &H1DE '478
    CMSG_CHAR_ENUM = &H37 '55
    CMSG_CHAR_CREATE = &H36 '54
    CMSG_CHAR_DELETE = &H38 '56
    CMSG_PLAYER_LOGIN = &H3D '61
End Enum

Enum CharCreateResponseCodes
    SUCCESS = &H28 '40 (Success)
    NAME_ALREADY_TAKEN = &H2B '43 (Name already taken)
End Enum

Enum AuthLoginCodes
    CHAR_LOGIN_FAILED = 0                       'Login failed
    CHAR_LOGIN_NO_WORLD = 1                     'World server is down
    CHAR_LOGIN_DUPLICATE_CHARACTER = 2          'A character with that name already exists
    CHAR_LOGIN_NO_INSTANCES = 3                 'No instance servers are available
    CHAR_LOGIN_DISABLED = 4                     'Login for that race and/or class is currently disabled
    CHAR_LOGIN_NO_CHARACTER = 5                 'Character not found
    CHAR_LOGIN_LOCKED_FOR_TRANSFER = 6
    CHAR_LOGIN_LOCKED_BY_BILLING = 7
End Enum

Enum AuthResponseCodes
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