import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { PackedDomainEvent, CircleStats } from "../types";
import "./CircleDetails.css";

const CircleDetails = (): JSX.Element => {
	const { circleId } = useParams<{ circleId: string }>();
	const [circle, setCircle] = useState<CircleStats | null>(null);
	const [loading, setLoading] = useState(true);
	const [secretKey, setSecretKey] = useState("");
	const [actionResult, setActionResult] = useState<string | null>(null);
	const navigate = useNavigate();
	const { authenticationToken } = useAuth();

	useEffect(() => {
		fetch(`/api/view/circle-stats/${circleId}`, {
			method: "POST",
			headers: { "Content-Type": "application/json" },
		})
			.then((res) => res.json())
			.then((data) => {
				setCircle(data);
				setLoading(false);
			})
			.catch((error) => {
				console.error("Error fetching circle details:", error);
				setLoading(false);
			});
	}, [circleId]);

	const handleRequest = async (endpoint: string): Promise<void> => {
		if (!circle) return;

		setActionResult(null);
		const requestId = crypto.randomUUID().replace(/-/g, "");
		const queryParams = new URLSearchParams({
			requestId,
			circleId: circle.circleId,
			secretKey,
		}).toString();

		try {
			const response = await fetch(`/api/circle-of-trust/${endpoint}?${queryParams}`, {
				method: "POST",
				headers: { Authorization: `Bearer ${authenticationToken}` },
			});

			if (response.ok) {
				const packedEvent: PackedDomainEvent = await response.json();
				setActionResult(packedEvent.eventName);
			} else {
				const errorText = await response.text();
				setActionResult(`Failed: ${errorText}`);
			}
		} catch (error) {
			console.error("Error:", error);
			setActionResult("Something went wrong.");
		}
	};

	if (loading) return <p className="loading-text">Loading details...</p>;
	if (!circle) return <p className="loading-text">Circle not found.</p>;
	return (
		<div className="circle-details">
			<h2 className="title">Circle Details</h2>
			<p><strong>ID:</strong> {circle.circleId}</p>
			<p><strong>Title:</strong> {circle.title}</p>
			<p><strong>Owner:</strong> {circle.owner}</p>
			<p><strong>Betrayed:</strong> {circle.isBetrayed ? "Yes" : "No"}</p>
			<p><strong>Members:</strong> {circle.members}</p>

			<div className="action-form">
				<input
					className="input"
					type="text"
					placeholder="Secret Key"
					value={secretKey}
					onChange={(e) => setSecretKey(e.target.value)}
				/>
				<button className="button button-yellow" onClick={() => handleRequest("join-circle")}>
					Join Circle
				</button>
				<button className="button button-red" onClick={() => handleRequest("betray-circle")}>
					Betray Circle
				</button>
			</div>

			{actionResult && <p className="action-result">{actionResult}</p>}

			<button onClick={() => navigate(-1)} className="back-button">
				Back to Leaderboard
			</button>
		</div>
	);
};

export default CircleDetails;
