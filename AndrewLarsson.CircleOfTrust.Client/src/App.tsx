import './App.css';
import { useState, useEffect } from "react";

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
	const [idToken, setIdToken] = useState<string | null>(null);
	const [response, setResponse] = useState<PackedDomainEvent | null>(null);
	const [googleLoaded, setGoogleLoaded] = useState(false); // Track script load

	const apiUrl = "/api/circle-of-trust";

	// Load Google API script
	useEffect(() => {
		const script = document.createElement("script");
		script.src = "https://accounts.google.com/gsi/client";
		script.async = true;
		script.defer = true;
		script.onload = () => setGoogleLoaded(true); // Mark script as loaded
		document.body.appendChild(script);
	}, []);

	// Google Sign-In Callback
	const handleCredentialResponse = (response: google.accounts.id.CredentialResponse) => {
		setIdToken(response.credential);
	};

	// Wait for script to load before initializing Google Sign-In
	useEffect(() => {
		if (googleLoaded && window.google?.accounts) {
			window.google.accounts.id.initialize({
				client_id: "68033942219-fshhuhthacrvi256dmca0ok4relukd76.apps.googleusercontent.com",
				callback: handleCredentialResponse,
			});
			const buttonContainer = document.getElementById("google-signin-btn");
			if (buttonContainer) {
				window.google.accounts.id.renderButton(
					buttonContainer as HTMLElement,
					{ theme: "outline", size: "large", type: "standard" }
				);
			}
		}
	}, [googleLoaded]); // Run only after script is fully loaded

	// API Request Function (Uses Authorization Header)
	const handleRequest = async (endpoint: string, payload: Record<string, string>): Promise<void> => {
		if (!idToken) {
			alert("Please sign in first.");
			return;
		}

		const requestId = crypto.randomUUID().replace(/-/g, "");

		// Convert payload into URL-encoded query parameters
		const queryParams = new URLSearchParams({ requestId, ...payload }).toString();

		const res = await fetch(`${apiUrl}/${endpoint}?${queryParams}`, {
			method: "POST",
			headers: {
				"Authorization": `Bearer ${idToken}`
			},
		});

		const data: PackedDomainEvent = await res.json();
		try {
			data.body = JSON.parse(data.body as string);
		} catch {
			// Keep it as a string if parsing fails
		}
		setResponse(data);
	};

	return (
		<div className="p-4 space-y-4">
			<h1 className="text-xl font-bold">Circle of Trust</h1>

			{/* Google Sign-In */}
			{!idToken && (
				<div className="border p-4 rounded-lg">
					<h2 className="font-semibold">Sign In</h2>
					{!idToken ? (
						<div id="google-signin-btn"></div>
					) : (
						<p className="mt-2 text-green-500">Signed in</p>
					)}
				</div>
			)}

			{/* Only show these sections if signed in */}
			{idToken && (
				<>
					{/* Claim Circle */}
					<div className="border p-4 rounded-lg">
						<h2 className="font-semibold">Claim Circle</h2>
						<input id="claimTitle" className="border p-2 w-full" type="text" placeholder="Title" />
						<input id="claimSecretKey" className="border p-2 w-full mt-2" type="text" placeholder="Secret Key" />
						<button
							className="bg-green-500 text-white p-2 rounded mt-2 w-full"
							onClick={() => handleRequest("claim-circle", {
								title: (document.getElementById("claimTitle") as HTMLInputElement).value,
								secretKey: (document.getElementById("claimSecretKey") as HTMLInputElement).value
							})}
						>
							Claim
						</button>
					</div>

					{/* Join Circle */}
					<div className="border p-4 rounded-lg">
						<h2 className="font-semibold">Join Circle</h2>
						<input id="joinCircleId" className="border p-2 w-full" type="text" placeholder="Circle ID" />
						<input id="joinSecretKey" className="border p-2 w-full mt-2" type="text" placeholder="Secret Key" />
						<button
							className="bg-yellow-500 text-white p-2 rounded mt-2 w-full"
							onClick={() => handleRequest("join-circle", {
								circleId: (document.getElementById("joinCircleId") as HTMLInputElement).value,
								secretKey: (document.getElementById("joinSecretKey") as HTMLInputElement).value
							})}
						>
							Join
						</button>
					</div>

					{/* Betray Circle */}
					<div className="border p-4 rounded-lg">
						<h2 className="font-semibold">Betray Circle</h2>
						<input id="betrayCircleId" className="border p-2 w-full" type="text" placeholder="Circle ID" />
						<input id="betraySecretKey" className="border p-2 w-full mt-2" type="text" placeholder="Secret Key" />
						<button
							className="bg-red-500 text-white p-2 rounded mt-2 w-full"
							onClick={() => handleRequest("betray-circle", {
								circleId: (document.getElementById("betrayCircleId") as HTMLInputElement).value,
								secretKey: (document.getElementById("betraySecretKey") as HTMLInputElement).value
							})}
						>
							Betray
						</button>
					</div>
				</>
			)}

			{/* Display Response */}
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
