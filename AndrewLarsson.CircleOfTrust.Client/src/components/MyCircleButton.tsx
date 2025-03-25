import { useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { CircleStats } from "../types";

const MyCircleButton = (): JSX.Element => {
	const navigate = useNavigate();
	const { authenticationToken } = useAuth();
	const [myCircle, setMyCircle] = useState<CircleStats | null>(null);
	const [loading, setLoading] = useState(true);
	const refreshKey = (useLocation().state as { refreshKey?: string })?.refreshKey;

	useEffect(() => {
		if (!authenticationToken) {
			return;
		}
		const fetchMyCircle = async () => {
			try {
				const response = await fetch("/api/view/my-circle-stats", {
					headers: {
						Authorization: `Bearer ${authenticationToken}`,
					},
				});
				if (!response.ok) throw new Error();
				const myCircleData: CircleStats = await response.json();
				setMyCircle(myCircleData);
			} catch {
				setMyCircle(null);
			} finally {
				setLoading(false);
			}
		};
		setLoading(true);
		fetchMyCircle();
	}, [authenticationToken, refreshKey]);

	const handleClick = () => {
		if (myCircle) {
			navigate(`/circle/${myCircle.circleId}`);
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
			href={myCircle ? `/circle/${myCircle.circleId}` : "/claim-circle"}
			className="nav-item"
			onClick={(e) => {
				e.preventDefault();
				handleClick();
			}}
		>
			{myCircle ? "My Circle" : "Claim Circle"}
		</a>
	);
};

export default MyCircleButton;
