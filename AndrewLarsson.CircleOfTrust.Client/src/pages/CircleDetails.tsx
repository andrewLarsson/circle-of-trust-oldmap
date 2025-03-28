import { useEffect, useState, useCallback } from "react";
import { useParams, useLocation } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { PackedDomainEvent, CircleStats } from "../types";
import User from "../components/User";
import "./CircleDetails.css";

const CircleDetails = (): JSX.Element => {
	const { circleId } = useParams<{ circleId: string }>();
	const [circle, setCircle] = useState<CircleStats | null>(null);
	const [loading, setLoading] = useState(true);
	const [secretKey, setSecretKey] = useState("");
	const [actionResult, setActionResult] = useState<string | null>(null);
	const syncTokenData = (useLocation().state as { syncToken?: string })?.syncToken;
	const [syncToken, setSyncToken] = useState<string | null>(syncTokenData || null);
	const { authenticationToken } = useAuth();

	const fetchCircle = useCallback(async (syncToken?: string) => {
		try {
			const headers: HeadersInit = {};
			if (syncToken) {
				headers["Synchronization-Token"] = syncToken;
			}
			const response = await fetch(`/api/view/circle-stats/${circleId}`, {
				headers
			});
			if (!response.ok) {
				console.warn("Failed to fetch circle.");
				return;
			}
			const circleData: CircleStats = await response.json();
			setCircle(circleData);
		} catch (error) {
			console.error("Error fetching circle:", error);
		} finally {
			setLoading(false);
		}
	}, [circleId]);

	useEffect(() => {
		fetchCircle(syncToken || undefined);
	}, [fetchCircle]); // eslint-disable-line react-hooks/exhaustive-deps

	const handleRequest = async (action: string): Promise<void> => {
		if (!circle) return;
		setActionResult(null);
		try {
			const requestId = crypto.randomUUID().replace(/-/g, "");
			const queryParams = new URLSearchParams({
				requestId,
				circleId: circle.circleId,
				secretKey
			}).toString();
			const response = await fetch(`/api/circle-of-trust/${action}?${queryParams}`, {
				method: "POST",
				headers: {
					Authorization: `Bearer ${authenticationToken}`
				}
			});
			if (!response.ok) {
				const errorText = await response.text();
				setActionResult(`Failed: ${errorText}`);
				return;
			}
			const eventData: PackedDomainEvent = await response.json();
			setActionResult(eventData.eventName);
			const syncTokenActionData = response.headers.get("Synchronization-Token");
			setSyncToken(syncTokenActionData);
			await fetchCircle(syncTokenActionData || undefined);
		} catch (error) {
			console.error("Error:", error);
			setActionResult("Something went wrong.");
		}
	};

	if (loading) return <p className="loading-text">Loading details...</p>;
	if (!circle) return <p className="loading-text">Circle not found.</p>;
	return (
		<div className="container">
			<h2 className="title">Circle Details</h2>
			<p><strong>ID:</strong> {circle.circleId}</p>
			<p><strong>Title:</strong> {circle.title}</p>
			<p><strong>Owner:</strong> <User userId={circle.owner} syncToken={syncToken || undefined } /></p>
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
				<button className="button button-green" onClick={() => handleRequest("join-circle")}>
					Join Circle
				</button>
				<button className="button button-red" onClick={() => handleRequest("betray-circle")}>
					Betray Circle
				</button>
			</div>
			{actionResult && <p className="action-result">{actionResult}</p>}
		</div>
	);
};

export default CircleDetails;
