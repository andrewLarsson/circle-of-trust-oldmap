import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import "./CircleDetails.css";

interface CircleStats {
	circleId: string;
	title: string;
	owner: string;
	isBetrayed: boolean;
	members: number;
}

const CircleDetails = (): JSX.Element => {
	const { circleId } = useParams<{ circleId: string }>(); // Get circle ID from URL
	const [circle, setCircle] = useState<CircleStats | null>(null);
	const [loading, setLoading] = useState(true);
	const navigate = useNavigate();

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
			<button onClick={() => navigate(-1)} className="back-button">
				Back to Leaderboard
			</button>
		</div>
	);
};

export default CircleDetails;
