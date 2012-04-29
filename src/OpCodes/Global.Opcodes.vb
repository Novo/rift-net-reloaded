<Flags()> Public Enum OpCodes As UShort
    'Realm Login
    SMSG_AUTH_CHALLENGE = &H1DD '477
    SMSG_AUTH_RESPONSE = &H1DF '279
    SMSG_CHAR_ENUM = &H3B '59
    SMSG_PONG = &H1CE '462
    SMSG_CHAR_CREATE = &H3A '58
    SMSG_CHAR_DELETE = &H3C '60


    CMSG_PING = &H1CD '461
    CMSG_AUTH_SESSION = &H1DE '478
    CMSG_CHAR_ENUM = &H37 '55
    CMSG_CHAR_CREATE = &H36 '54
    CMSG_CHAR_DELETE = &H38 '56
    CMSG_PLAYER_LOGIN = &H3D '61
End Enum

Enum AuthResponseCodes
    WRONG_CLIENT = &H6 '6
    AUTH_OK = &HC '12
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