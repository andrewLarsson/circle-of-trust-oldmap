import React, { useState } from "react";
import './App.css';
//import logo from './assets/Subtle Secrets.png';

const App: React.FC = () => {
	const [message, setMessage] = useState("");

	const callApi = async () => {
		const requestId = crypto.randomUUID(); // Generate a unique requestId
		try {
			const response = await fetch(`/api/circle-of-trust/test?requestId=${requestId}`, { method: "GET" });
			if (response.ok) {
				setMessage(`Request sent successfully! Request ID: ${requestId}`);
			} else {
				setMessage(`Error: ${response.statusText}`);
			}
		} catch (error) {
			setMessage(`Error: ${String(error)}`);
		}
	};

	return (
		<div className="p-4">
			<button onClick={callApi} className="px-4 py-2 bg-blue-500 text-white rounded">
				Call API
			</button>
			{message && <p className="mt-2">{message}</p>}
		</div>
	);
};

export default App;
