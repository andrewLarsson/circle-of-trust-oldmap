CREATE DATABASE CircleOfTrustView;
\c CircleOfTrustView;

CREATE TABLE IdempotentTransactions (
	IdempotencyKey VARCHAR(255) PRIMARY KEY
);

CREATE TABLE CircleStats (
	CircleId VARCHAR(255) PRIMARY KEY,
	Title TEXT NOT NULL,
	Owner VARCHAR(255) NOT NULL,
	IsBetrayed BOOLEAN NOT NULL,
	Members INT NOT NULL
);
CREATE INDEX IX_CircleStats_Owner ON CircleStats(Owner);
CREATE INDEX IX_CircleStats_IsBetrayed ON CircleStats(IsBetrayed);

CREATE TABLE UserStats (
	UserId VARCHAR(255) PRIMARY KEY,
	CircleId VARCHAR(255) NOT NULL,
	MemberOfCircles INT NOT NULL,
	MemberOfNonbetrayedCircles INT NOT NULL,
	MemberOfBetrayedCircles INT NOT NULL
);
CREATE INDEX IX_UserStats_CircleId ON UserStats(CircleId);

CREATE TABLE UserStatsCircleMembers (
	UserIdCircleId VARCHAR(255) PRIMARY KEY,
	UserId VARCHAR(255) NOT NULL,
	CircleId VARCHAR(255) NOT NULL
);
CREATE INDEX IX_UserStatsCircleMembers_UserId ON UserStatsCircleMembers(UserId);
CREATE INDEX IX_UserStatsCircleMembers_CircleId ON UserStatsCircleMembers(CircleId);

CREATE TABLE CircleLeaderboardContender (
	CircleLeaderboardContenderId SERIAL PRIMARY KEY, -- Auto-incrementing primary key
	CircleId UUID NOT NULL,
	Members INT NOT NULL
);
CREATE INDEX IX_CircleLeaderboardContender_CircleId ON CircleLeaderboardContender(CircleId);
CREATE INDEX IX_CircleLeaderboardContender_Members ON CircleLeaderboardContender(Members);
