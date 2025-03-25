import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { CircleStats } from "../types";
import "./Leaderboard.css";

const Leaderboard = (): JSX.Element => {
	const [leaderboard, setLeaderboard] = useState<CircleStats[]>([]);
	const [loading, setLoading] = useState(true);
	const navigate = useNavigate();

	useEffect(() => {
		const fetchLeaderboard = async () => {
			try {
				const response = await fetch("/api/view/leaderboard");
				const leaderboardData: CircleStats[] = await response.json();
				setLeaderboard(leaderboardData);
			} catch (error) {
				console.error("Error fetching leaderboard:", error);
			} finally {
				setLoading(false);
			}
		};
		fetchLeaderboard();
	}, []);

	return (
		<div className="leaderboard-container">
			<h2 className="leaderboard-title">Leaderboard</h2>
			{loading ? (
				<p className="loading-text">Loading leaderboard...</p>
			) : (
				<table className="leaderboard-table">
					<thead>
						<tr>
							<th>Circle ID</th>
							<th>Title</th>
							<th>Owner</th>
							<th>Betrayed</th>
							<th>Members</th>
						</tr>
					</thead>
					<tbody>
						{leaderboard.map((stat) => (
							<tr key={stat.circleId} className="clickable-row" onClick={() => navigate(`/circle/${stat.circleId}`)}>
								<td>{stat.circleId}</td>
								<td>{stat.title}</td>
								<td>{stat.owner}</td>
								<td>{stat.isBetrayed ? "Yes" : "No"}</td>
								<td>{stat.members}</td>
							</tr>
						))}
					</tbody>
				</table>
			)}
		</div>
	);
};

export default Leaderboard;
