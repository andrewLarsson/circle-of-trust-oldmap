import { useAuth } from "../auth/useAuth";
import "./CircleActions.css";

const CircleActions = (): JSX.Element => {
	const { authenticationToken } = useAuth();

	const handleRequest = async (endpoint: string, payload: Record<string, string>): Promise<void> => {
		const requestId = crypto.randomUUID().replace(/-/g, "");
		const queryParams = new URLSearchParams({ requestId, ...payload }).toString();
		await fetch(`/api/circle-of-trust/${endpoint}?${queryParams}`, {
			method: "POST",
			headers: { Authorization: `Bearer ${authenticationToken}` },
		});
	};

	return (
		<>
			{/* Claim Circle */}
			<div className="card">
				<h2 className="card-title">Claim Circle</h2>
				<input id="claimTitle" className="input" type="text" placeholder="Title" />
				<input id="claimSecretKey" className="input" type="text" placeholder="Secret Key" />
				<button
					className="button button-green"
					onClick={() =>
						handleRequest("claim-circle", {
							title: (document.getElementById("claimTitle") as HTMLInputElement).value,
							secretKey: (document.getElementById("claimSecretKey") as HTMLInputElement).value,
						})
					}
				>
					Claim
				</button>
			</div>

			{/* Join Circle */}
			<div className="card">
				<h2 className="card-title">Join Circle</h2>
				<input id="joinCircleId" className="input" type="text" placeholder="Circle ID" />
				<input id="joinSecretKey" className="input" type="text" placeholder="Secret Key" />
				<button
					className="button button-yellow"
					onClick={() =>
						handleRequest("join-circle", {
							circleId: (document.getElementById("joinCircleId") as HTMLInputElement).value,
							secretKey: (document.getElementById("joinSecretKey") as HTMLInputElement).value,
						})
					}
				>
					Join
				</button>
			</div>

			{/* Betray Circle */}
			<div className="card">
				<h2 className="card-title">Betray Circle</h2>
				<input id="betrayCircleId" className="input" type="text" placeholder="Circle ID" />
				<input id="betraySecretKey" className="input" type="text" placeholder="Secret Key" />
				<button
					className="button button-red"
					onClick={() =>
						handleRequest("betray-circle", {
							circleId: (document.getElementById("betrayCircleId") as HTMLInputElement).value,
							secretKey: (document.getElementById("betraySecretKey") as HTMLInputElement).value,
						})
					}
				>
					Betray
				</button>
			</div>
		</>
	);
};

export default CircleActions;
