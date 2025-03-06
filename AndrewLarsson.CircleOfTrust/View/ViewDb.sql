CREATE DATABASE CircleOfTrustView;
\c CircleOfTrustView;

CREATE TABLE IdempotentTransactions (
	IdempotencyKey VARCHAR(255) PRIMARY KEY
);

CREATE TABLE PlayerStats (
	PlayerId UUID PRIMARY KEY,
	Username VARCHAR(255) NOT NULL,
	HasCircle BOOLEAN NOT NULL,
	MemberOfCircles INT NOT NULL,
	BetrayedCircles INT NOT NULL
);
CREATE INDEX IX_PlayerStats_Username ON PlayerStats(Username);
CREATE INDEX IX_PlayerStats_HasCircle ON PlayerStats(HasCircle);

CREATE TABLE CircleStats (
	CircleId VARCHAR(255) PRIMARY KEY,
	Title TEXT NOT NULL,
	Owner VARCHAR(255) NOT NULL,
	IsBetrayed BOOLEAN NOT NULL,
	Members INT NOT NULL
);
CREATE INDEX IX_CircleStats_Owner ON CircleStats(Owner);
CREATE INDEX IX_CircleStats_IsBetrayed ON CircleStats(IsBetrayed);

CREATE TABLE CircleLeaderboardContender (
	CircleLeaderboardContenderId SERIAL PRIMARY KEY, -- Auto-incrementing primary key
	CircleId UUID NOT NULL,
	Members INT NOT NULL
);
CREATE INDEX IX_CircleLeaderboardContender_CircleId ON CircleLeaderboardContender(CircleId);
CREATE INDEX IX_CircleLeaderboardContender_Members ON CircleLeaderboardContender(Members);
