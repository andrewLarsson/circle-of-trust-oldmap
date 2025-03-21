export interface AggregateRootAddress {
	domain: string;
	aggregateRoot: string;
	aggregateRootId: string;
}

export interface PackedDomainEvent {
	domainMessageId: string;
	address: AggregateRootAddress;
	eventName: string;
	body: string;
}

export interface CircleStats {
	circleId: string;
	title: string;
	owner: string;
	isBetrayed: boolean;
	members: number;
}
