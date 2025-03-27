import { useEffect, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { CircleStats } from "../types";

const MyCircleButton = (): JSX.Element => {
	const navigate = useNavigate();
	const { authenticationToken } = useAuth();
	const location = useLocation();
	const [myCircleId, setMyCircleId] = useState<string | null>(null);
	const syncToken = (location.state as { syncToken?: string })?.syncToken;

	useEffect(() => {
		const myCircleIdData = (location.state as { myCircleId?: string })?.myCircleId;
		if (myCircleIdData) {
			setMyCircleId(myCircleIdData);
		}
	}, [location.state]);

	useEffect(() => {
		if (myCircleId || !authenticationToken) {
			return;
		}
		const fetchMyCircle = async () => {
			try {
				const headers: HeadersInit = {
					Authorization: `Bearer ${authenticationToken}`
				};
				if (syncToken) {
					headers["Synchronization-Token"] = syncToken;
				}
				const response = await fetch("/api/view/my-circle-stats", {
					headers
				});
				if (!response.ok) throw new Error();
				const myCircleData: CircleStats = await response.json();
				setMyCircleId(myCircleData.circleId);
			} catch {
				setMyCircleId(null);
			}
		};
		fetchMyCircle();
	}, [authenticationToken]); // eslint-disable-line react-hooks/exhaustive-deps

	const handleClick = () => {
		if (myCircleId) {
			navigate(`/circle/${myCircleId}`);
		} else {
			navigate("/claim-circle");
		}
	};

	return (
		<a
			href={myCircleId ? `/circle/${myCircleId}` : "/claim-circle"}
			className="nav-item"
			onClick={(e) => {
				e.preventDefault();
				handleClick();
			}}
		>
			{myCircleId ? "My Circle" : "Claim Circle"}
		</a>
	);
};

export default MyCircleButton;
