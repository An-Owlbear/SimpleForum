﻿CREATE TABLE `OutgoingServerTokens` (
    `OutgoingServerTokenID` int NOT NULL AUTO_INCREMENT,
    `Address` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ApiAddress` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CrossConnectionAddress` varchar(255) CHARACTER SET  utf8mb4 NOT NULL,
    `TOKEN` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_OutgoingServerTokens` PRIMARY KEY (`OutgoingServerTokenID`)
);

CREATE TABLE `IncomingServerTokens` (
    `IncomingServerTokenID` int NOT NULL AUTO_INCREMENT,
    `Address` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `ApiAddress` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `CrossConnectionAddress` varchar(255) CHARACTER SET  utf8mb4 NOT NULL,
    `TOKEN` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_IncomingServerTokens` PRIMARY KEY (`IncomingServerTokenID`)
);

CREATE TABLE `Users` (
    `UserID` int NOT NULL AUTO_INCREMENT,
    `Username` longtext CHARACTER SET utf8mb4 NULL,
    `Email` longtext CHAR SET utf8mb4 NULL,
    `Password` longtext CHARACTER SET utf8mb4 NULL,
    `SignupDate` datetime(6) NOT NULL,
    `ServerID` int NULL,
    `Bio` longtext CHARACTER SET utf8mb4 NULL,
    `Activated` tinyint(1) NOT NULL DEFAULT FALSE,
    `Role` longtext CHARACTER SET utf8mb4 NULL,
    `CommentsLocked` tinyint(1) NOT NULL DEFAULT FALSE,
    `Muted` tinyint(1) NOT NULL DEFAULT FALSE,
    `MuteReason` longtext CHARACTER SET utf8mb4 NULL,
    `Banned` tinyint(1) NOT NULL DEFAULT FALSE,
    `BanReason` longtext CHARACTER SET utf8mb4 NULL,
    `Deleted` tinyint(1) NOT NULL DEFAULT FALSE,
    CONSTRAINT `PK_Users` PRIMARY KEY (`UserID`),
    CONSTRAINT `FK_Users_IncomingServerTokens_ServerID` FOREIGN KEY (`ServerID`) 
        REFERENCES `IncomingServerTokens` (`IncomingServerTokenID`) ON DELETE RESTRICT
);

CREATE TABLE `Threads` (
    `ThreadID` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NULL,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `DatePosted` datetime(6) NOT NULL,
    `UserID` int NOT NULL,
    `Pinned` tinyint(1) NOT NULL DEFAULT FALSE,
    `Locked` tinyint(1) NOT NULL DEFAULT FALSE,
    `LockedBy` longtext CHARACTER SET utf8mb4 NULL,
    `Deleted` tinyint(1) NOT NULL DEFAULT FALSE,
    `DeletedBy` longtext CHARACTER SET utf8mb4 NULL,
    `DeleteReason` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Threads` PRIMARY KEY (`ThreadID`),
    CONSTRAINT `FK_Threads_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE TABLE `Comments` (
    `CommentID` int NOT NULL AUTO_INCREMENT,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `DatePosted` datetime(6) NOT NULL,
    `Deleted` tinyint(1) NOT NULL DEFAULT FALSE,
    `DeletedBy` longtext CHARACTER SET utf8mb4 NULL,
    `DeleteReason` longtext CHARACTER SET utf8mb4 NULL,
    `UserID` int NOT NULL,
    `ThreadID` int NOT NULL,
    CONSTRAINT `PK_Comments` PRIMARY KEY (`CommentID`),
    CONSTRAINT `FK_Comments_Threads_ThreadID` FOREIGN KEY (`ThreadID`) REFERENCES `Threads` (`ThreadID`) ON DELETE CASCADE,
    CONSTRAINT `FK_Comments_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE TABLE `UserComments` (
    `UserCommentID` int NOT NULL AUTO_INCREMENT,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `DatePosted` datetime(6) NOT NULL,
    `Deleted` tinyint(1) NOT NULL DEFAULT FALSE,
    `DeletedBy` longtext CHARACTER SET utf8mb4 NULL,
    `DeleteReason` longtext CHARACTER SET utf8mb4 NULL,
    `UserID` int NOT NULL,
    `UserPageID` int NOT NULL,
    CONSTRAINT `PK_UserComments` PRIMARY KEY (`UserCommentID`),
    CONSTRAINT `FK_UserComments_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE,
    CONSTRAINT `FK_UserComments_Users_UserPageID` FOREIGN KEY (`UserPageID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE TABLE `RemoteAuthTokens` (
    `Token` varchar(255) CHARACTER SET utf8mb4 NULL,
    `ValidUntil` datetime(6) NOT NULL,
    `UserID` int NOT NULL,
    CONSTRAINT `PK_RemoteAuthTokens` PRIMARY KEY (`Token`),
    CONSTRAINT `FK_RemoteAuthTokens_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE TABLE `EmailCodes` (
    `Code` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Type` longtext CHARACTER SET utf8mb4 NULL,
    `Valid` tinyint(1) NOT NULL DEFAULT FALSE,
    `DateCreated` datetime(6) NOT NULL,
    `ValidUntil` datetime(6) NOT NULL,
    `UserID` int NOT NULL,
    CONSTRAINT `PK_EmailCodes` PRIMARY KEY (`Code`),
    CONSTRAINT `FK_EmailCodes_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE TABLE `Notifications` (
    `NotificationID` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NULL,
    `Content` longtext CHARACTER SET utf8mb4 NULL,
    `DateCreated` datetime(6) NOT NULL,
    `Read` tinyint(1) NOT NULL DEFAULT FALSE,
    `UserID` int NOT NULL,
    CONSTRAINT `PK_Notifications` PRIMARY KEY (`NotificationID`),
    CONSTRAINT `FK_Notifications_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE TABLE `TempApiTokens` (
    `Token` varchar(128) CHARACTER SET utf8mb4 NOT NULL,
    `ValidUntil` datetime(6) NOT NULL,
    `UserID` int NOT NULL,
    CONSTRAINT `PK_TempApiTokens` PRIMARY KEY (`Token`),
    CONSTRAINT `FK_TempApiTokens_Users_UserID` FOREIGN KEY (`UserID`) REFERENCES `Users` (`UserID`) ON DELETE CASCADE
);

CREATE INDEX `IX_Users_ServerID` on `Users` (`ServerID`);
CREATE INDEX `IX_Comments_ThreadID` ON `Comments` (`ThreadID`);
CREATE INDEX `IX_Comments_UserID` ON `Comments` (`UserID`);
CREATE INDEX `IX_Comments_DatePosted` ON `Comments` (`DatePosted`);
CREATE INDEX `IX_Threads_UserID` ON `Threads` (`UserID`);
CREATE INDEX `IX_Threads_DatePosted` ON `Threads` (`DatePosted`);
CREATE INDEX `IX_UserComments_UserID` ON `UserComments` (`UserID`);
CREATE INDEX `IX_UserComments_UserPageID` ON `UserComments` (`UserPageID`);
CREATE INDEX `IX_UserComments_DatePosted` ON `UserComments` (`DatePosted`);
CREATE INDEX `IX_RemoteAuthTokens_UserID` ON `RemoteAuthTokens` (`UserID`);
CREATE INDEX `IX_EmailCodes_UserID` ON `EmailCodes` (`UserID`);
CREATE INDEX `IX_TempApiTokens_Token` ON `TempApiTokens` (`Token`)