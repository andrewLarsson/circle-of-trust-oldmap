import { useEffect, useState } from "react";
import { UserStats } from "../types";

interface UserProps {
	userId: string;
	syncToken?: string;
}

const User = ({ userId, syncToken }: UserProps): JSX.Element => {
	const [user, setUser] = useState<UserStats | null>(null);
	const [loading, setLoading] = useState(true);

	useEffect(() => {
		const fetchUserStats = async () => {
			try {
				const headers: HeadersInit = {};
				if (syncToken) {
					headers["Synchronization-Token"] = syncToken;
				}
				const response = await fetch(`/api/view/user-stats/${userId}`, {
					headers,
				});
				if (!response.ok) {
					console.warn("Failed to fetch user.");
					return;
				}
				const userData: UserStats = await response.json();
				setUser(userData);
			} catch (error) {
				console.error("Error fetching user:", error);
			} finally {
				setLoading(false);
			}
		};
		fetchUserStats();
	}, [userId, syncToken]);

	if (loading) return <span>Loading...</span>;
	if (!user) return <span>User not available</span>;
	console.log(user);
	return (
		<span>
			{user.userId} [{user.memberOfCircles}, {user.memberOfNonbetrayedCircles}, {user.memberOfBetrayedCircles}]
		</span>
	);
};

export default User;
