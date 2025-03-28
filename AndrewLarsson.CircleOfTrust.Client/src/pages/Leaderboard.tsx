import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { CircleStats } from "../types";
import User from "../components/User";
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
		<div className="container">
			<h2 className="title">Leaderboard</h2>
			{loading ? (
				<p className="loading-text">Loading leaderboard...</p>
			) : (
				<table className="leaderboard-table">
					<thead>
						<tr>
							<th>Title</th>
							<th>Owner</th>
							<th>Members</th>
						</tr>
					</thead>
					<tbody>
						{leaderboard.map((circle) => (
							<tr
								key={circle.circleId}
								className={`clickable-row ${circle.isBetrayed ? "betrayed" : ""}`}
								onClick={() => navigate(`/circle/${circle.circleId}`)}
							>
								<td>{circle.title}</td>
								<td><User userId={circle.owner} /></td>
								<td>{circle.members}</td>
							</tr>
						))}
					</tbody>
				</table>
			)}
		</div>
	);
};

export default Leaderboard;
