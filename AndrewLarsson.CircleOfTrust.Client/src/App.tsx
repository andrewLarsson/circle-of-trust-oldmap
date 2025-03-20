import './App.css';
import { useEffect } from "react";
import { Routes, Route, useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "./auth/useAuth";
import Layout from "./Layout";
import Login from "./Login";
import Leaderboard from "./pages/Leaderboard";
import CircleDetails from "./pages/CircleDetails";
import CircleActions from './pages/CircleActions';

const App = (): JSX.Element => {
	const navigate = useNavigate();
	const location = useLocation();
	const { isAuthenticated } = useAuth();

	useEffect(() => {
		if (!isAuthenticated && location.pathname !== "/login") {
			navigate("/login", { replace: true });
		}
	}, [isAuthenticated, location.pathname, navigate]);

	return (
		<Layout>
			<Routes>
				<Route path="/" element={<Leaderboard />} />
				<Route path="/login" element={<Login />} />
				<Route path="/circle/:circleId" element={<CircleDetails />} />
				<Route path="/actions" element={<CircleActions />} />
			</Routes>
		</Layout>
	);
};

export default App;
