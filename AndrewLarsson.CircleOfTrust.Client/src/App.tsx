import './App.css';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "./Layout";
import Leaderboard from "./pages/Leaderboard";
import CircleDetails from "./pages/CircleDetails";
import OldApp from './pages/OldApp';

const App = (): JSX.Element => {
	return (
		<Router>
			<Layout>
				<Routes>
					<Route path="/" element={<Leaderboard />} />
					<Route path="/circle/:circleId" element={<CircleDetails />} />
					<Route path="/app" element={<OldApp />} />
				</Routes>
			</Layout>
		</Router>
	);
};

export default App;
