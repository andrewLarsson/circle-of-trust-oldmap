import './App.css';
//import logo from './assets/Subtle Secrets.png';

import { useState } from "react";

type AggregateRootAddress = {
	domain: string;
	aggregateRoot: string;
	aggregateRootId: string;
};

type PackedDomainEvent = {
	domainMessageId: string;
	address: AggregateRootAddress;
	eventName: string;
	body: string | Record<string, unknown>;
};

const App = (): JSX.Element => {
	const [userId, setUserId] = useState<string>("");
	const [userToken, setUserToken] = useState<string>("");
	const [response, setResponse] = useState<PackedDomainEvent | null>(null);

	const apiUrl = "/api/circle-of-trust";

	const handleSignIn = async (): Promise<void> => {
		const res = await fetch(`${apiUrl}/sign-in?userId=${userId}`, {
			method: "POST",
		});
		const data = await res.text();
		setUserToken(data);
	};

	const handleRequest = async (endpoint: string, payload: Record<string, string>): Promise<void> => {
		const requestId = crypto.randomUUID().replace(/-/g, "");
		const queryParams = new URLSearchParams({ requestId, userToken, ...payload }).toString();
		const res = await fetch(`${apiUrl}/${endpoint}?${queryParams}`, {
			method: "POST"
		});
		const data: PackedDomainEvent = await res.json();
		try {
			data.body = JSON.parse(data.body as string);
		} catch {
			// If parsing fails, keep it as a string
		}
		setResponse(data);
	};

	return (
		<div className="p-4 space-y-4">
			<h1 className="text-xl font-bold">Circle of Trust API</h1>

			{/* Sign In */}
			<div className="border p-4 rounded-lg">
				<h2 className="font-semibold">Sign Up</h2>
				<input
					className="border p-2 w-full"
					type="text"
					placeholder="User ID"
					value={userId}
					onChange={(e) => setUserId(e.target.value)}
				/>
				<button
					className="bg-blue-500 text-white p-2 rounded mt-2 w-full"
					onClick={handleSignIn}
				>
					Sign In
				</button>
				{userToken && <p className="mt-2">User Token: {userToken}</p>}
			</div>

			{/* Claim Circle */}
			<div className="border p-4 rounded-lg">
				<h2 className="font-semibold">Claim Circle</h2>
				<input className="border p-2 w-full" type="text" placeholder="Title" id="claimTitle" />
				<input className="border p-2 w-full mt-2" type="text" placeholder="Secret Key" id="claimSecretKey" />
				<button className="bg-green-500 text-white p-2 rounded mt-2 w-full" onClick={() => handleRequest("claim-circle", {
					title: (document.getElementById("claimTitle") as HTMLInputElement).value,
					secretKey: (document.getElementById("claimSecretKey") as HTMLInputElement).value
				})}>Claim</button>
			</div>

			{/* Join Circle */}
			<div className="border p-4 rounded-lg">
				<h2 className="font-semibold">Join Circle</h2>
				<input className="border p-2 w-full" type="text" placeholder="Circle ID" id="joinCircleId" />
				<input className="border p-2 w-full mt-2" type="text" placeholder="Secret Key" id="joinSecretKey" />
				<button className="bg-yellow-500 text-white p-2 rounded mt-2 w-full" onClick={() => handleRequest("join-circle", {
					circleId: (document.getElementById("joinCircleId") as HTMLInputElement).value,
					secretKey: (document.getElementById("joinSecretKey") as HTMLInputElement).value
				})}>Join</button>
			</div>

			{/* Betray Circle */}
			<div className="border p-4 rounded-lg">
				<h2 className="font-semibold">Betray Circle</h2>
				<input className="border p-2 w-full" type="text" placeholder="Circle ID" id="betrayCircleId" />
				<input className="border p-2 w-full mt-2" type="text" placeholder="Secret Key" id="betraySecretKey" />
				<button className="bg-red-500 text-white p-2 rounded mt-2 w-full" onClick={() => handleRequest("betray-circle", {
					circleId: (document.getElementById("betrayCircleId") as HTMLInputElement).value,
					secretKey: (document.getElementById("betraySecretKey") as HTMLInputElement).value
				})}>Betray</button>
			</div>

			{response && (
				<div className="border p-4 rounded-lg">
					<h2 className="font-semibold">Response</h2>
					<pre className="bg-gray-100 p-2 rounded">{JSON.stringify(response, null, 2)}</pre>
				</div>
			)}
		</div>
	);
};

export default App;
