import { Link } from "react-router-dom";
import MyCircleButton from "./components/MyCircleButton";
import "./Layout.css";

const Layout = ({ children }: { children: React.ReactNode }): JSX.Element => {
	return (
		<div className="layout-container">
			<nav className="navbar">
				<h1 className="logo">Circle of Trust</h1>
				<div className="nav-links">
					<Link to="/" className="nav-item">Leaderboard</Link>
					<MyCircleButton />
				</div>
			</nav>

			<main className="content">{children}</main>
		</div>
	);
};

export default Layout;
