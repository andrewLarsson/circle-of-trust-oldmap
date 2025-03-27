import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { CircleStats, PackedDomainEvent } from "../types";
import "./ClaimCircle.css";

const ClaimCircle = (): JSX.Element => {
	const [title, setTitle] = useState("");
	const [secretKey, setSecretKey] = useState("");
	const [result, setResult] = useState<string | null>(null);
	const navigate = useNavigate();
	const { authenticationToken } = useAuth();

	const handleClaim = async () => {
		const requestId = crypto.randomUUID().replace(/-/g, "");
		const params = new URLSearchParams({
			requestId,
			title,
			secretKey
		}).toString();
		try {
			const response = await fetch(`/api/circle-of-trust/claim-circle?${params}`, {
				method: "POST",
				headers: {
					Authorization: `Bearer ${authenticationToken}`
				}
			});
			if (!response.ok) {
				const error = await response.text();
				setResult(`Failed to claim circle: ${error}`);
				return;
			}
			const eventData: PackedDomainEvent = await response.json();
			setResult(eventData.eventName);
			if (eventData.eventName !== "CircleClaimed") {
				return;
			}
			const syncToken = response.headers.get("Synchronization-Token");
			const headers: HeadersInit = {
				Authorization: `Bearer ${authenticationToken}`
			};
			if (syncToken) {
				headers["Synchronization-Token"] = syncToken;
			}
			const myCircleResponse = await fetch("/api/view/my-circle-stats", {
				headers
			});
			if (!myCircleResponse.ok) {
				return;
			}
			const myCircleData: CircleStats = await myCircleResponse.json();
			navigate(`/circle/${myCircleData.circleId}`, {
				state: {
					myCircleId: myCircleData.circleId,
					syncToken: syncToken
				}
			});
		} catch (error) {
			console.error("Error:", error);
			setResult("Something went wrong.");
		}
	};

	return (
		<div className="claim-circle">
			<h2 className="title">Claim Your Circle</h2>
			<div className="action-form">
				<input
					type="text"
					className="input"
					placeholder="Circle Title"
					value={title}
					onChange={(e) => setTitle(e.target.value)}
				/>
				<input
					type="text"
					className="input"
					placeholder="Secret Key"
					value={secretKey}
					onChange={(e) => setSecretKey(e.target.value)}
				/>
				<button className="button button-yellow" onClick={handleClaim}>
					Claim Circle
				</button>
			</div>
			{result && <p className="action-result">{result}</p>}
			<button className="back-button" onClick={() => navigate(-1)}>
				Back
			</button>
		</div>
	);
};

export default ClaimCircle;
