import './App.css';
import { useEffect } from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "./auth/useAuth";
import Layout from "./Layout";
import Login from "./Login";
import Leaderboard from "./pages/Leaderboard";
import ClaimCircle from "./pages/ClaimCircle";
import CircleDetails from "./pages/CircleDetails";

const App = (): JSX.Element => {
	const navigate = useNavigate();
	const location = useLocation();
	const { isAuthenticated } = useAuth();

	useEffect(() => {
		if (!isAuthenticated && location.pathname !== "/login") {
			navigate("/login", {
				replace: true,
				state: {
					from: location.pathname + location.search + location.hash
				}
			});
		}
	}, [isAuthenticated, navigate, location.pathname, location.search, location.hash]);

	return (
		<Layout>
			<Routes>
				<Route path="/" element={<Leaderboard />} />
				<Route path="/login" element={<Login />} />
				<Route path="/claim-circle" element={<ClaimCircle />} />
				<Route path="/circle/:circleId" element={<CircleDetails />} />
			</Routes>
		</Layout>
	);
};

export default App;
