Imports Localize = DotNetNuke.Services.Localization.Localization
Imports DotNetNuke.Entities.Host
Imports System.Net.Mail

Namespace DotNetNuke.Services.Mail

    Public Enum MessageType
        PasswordReminder
        ProfileUpdated
        UserRegistrationAdmin
        UserRegistrationPrivate
        UserRegistrationPublic
        UserRegistrationVerified
        UserUpdatedOwnPassword
    End Enum

End Namespace
