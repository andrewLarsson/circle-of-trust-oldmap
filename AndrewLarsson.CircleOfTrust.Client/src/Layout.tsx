import { Link } from "react-router-dom";
import MyCircleButton from "./components/MyCircleButton";
import logo from './assets/Circle of Trust Logo.png';
import "./Layout.css";

const Layout = ({ children }: { children: React.ReactNode }): JSX.Element => {
	return (
		<div className="layout-container">
			<div className="header">
				<img src={logo} alt="Circle of Trust Logo" className="logo" />
				<nav className="navbar">
					<h1>Circle of Trust</h1>
					<div className="nav-links">
						<Link to="/" className="nav-item">Leaderboard</Link>
						<MyCircleButton />
					</div>
				</nav>
			</div>
			<main className="content">{children}</main>
		</div>
	);
};

export default Layout;
