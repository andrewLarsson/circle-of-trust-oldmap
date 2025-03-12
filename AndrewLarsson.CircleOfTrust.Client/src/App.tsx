import React, { useState } from "react";
import './App.css';
//import logo from './assets/Subtle Secrets.png';

const App: React.FC = () => {
	const [responseData, setResponseData] = useState<object | null>(null);
	const [error, setError] = useState<string | null>(null);
	const [loading, setLoading] = useState(false);

	const callApi = async () => {
		const requestId = crypto.randomUUID(); // Generate a unique requestId
		setLoading(true);
		setError(null);
		setResponseData(null);

		try {
			const response = await fetch(`/api/circle-of-trust/test-with-response?requestId=${requestId}`, {
				method: "POST",
			});

			if (!response.ok) {
				throw new Error(`Error: ${response.status} ${response.statusText}`);
			}

			const data = await response.json();
			setResponseData(data);
		} catch (error) {
			setError(String(error));
		} finally {
			setLoading(false);
		}
	};

	return (
		<div className="p-4">
			<button onClick={callApi} className="px-4 py-2 bg-blue-500 text-white rounded" disabled={loading}>
				{loading ? "Loading..." : "Call API"}
			</button>

			{error && <p className="mt-2 text-red-500">{error}</p>}

			{responseData && (
				<pre className="mt-2 p-2 bg-gray-100 rounded text-sm">
					{JSON.stringify(responseData, null, 2)}
				</pre>
			)}
		</div>
	);
};

export default App;
