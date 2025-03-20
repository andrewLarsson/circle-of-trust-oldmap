import { Link } from "react-router-dom";
import "./Layout.css";

const Layout = ({ children }: { children: React.ReactNode }): JSX.Element => {
	return (
		<div className="layout-container">
			{/* Navigation Bar */}
			<nav className="navbar">
				<h1 className="logo">Circle of Trust</h1>
				<div className="nav-links">
					<Link to="/" className="nav-item">Leaderboard</Link>
					<Link to="/actions" className="nav-item">Actions</Link>
				</div>
			</nav>

			{/* Page Content */}
			<main className="content">{children}</main>
		</div>
	);
};

export default Layout;
