import { useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { CircleStats } from "../types";

const MyCircleButton = (): JSX.Element => {
	const navigate = useNavigate();
	const { authenticationToken } = useAuth();
	const [circleStats, setCircleStats] = useState<CircleStats | null>(null);
	const [loading, setLoading] = useState(true);
	const refreshKey = (useLocation().state as { refreshKey?: string })?.refreshKey;

	useEffect(() => {
		const fetchCircleStats = async () => {
			try {
				const response = await fetch("/api/view/my-circle-stats", {
					method: "POST",
					headers: {
						"Content-Type": "application/json",
						Authorization: `Bearer ${authenticationToken}`,
					},
				});

				if (!response.ok) throw new Error();
				const data = await response.json();
				setCircleStats(data ?? null);
			} catch {
				setCircleStats(null);
			} finally {
				setLoading(false);
			}
		};

		if (authenticationToken) {
			setLoading(true);
			fetchCircleStats();
		}
	}, [authenticationToken, refreshKey]);

	const handleClick = () => {
		if (circleStats?.circleId) {
			navigate(`/circle/${circleStats.circleId}`);
		} else {
			navigate("/claim-circle");
		}
	};

	if (loading) return (
		<a
			href="/"
			className="nav-item"
			onClick={(e) => {
				e.preventDefault();
			}}
		>
			My Circle
		</a>
	);
	return (
		<a
			href={circleStats?.circleId ? `/circle/${circleStats.circleId}` : "/claim-circle"}
			className="nav-item"
			onClick={(e) => {
				e.preventDefault();
				handleClick();
			}}
		>
			{circleStats?.circleId ? "My Circle" : "Claim Circle"}
		</a>
	);
};

export default MyCircleButton;
