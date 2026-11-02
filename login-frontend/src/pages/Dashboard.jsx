
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getUserProfile, logoutUser } from "../services/authService";

function Dashboard() {
    const navigate = useNavigate();
    const [profile, setProfile] = useState(null);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const response = await getUserProfile();
                setProfile(response.user);
            } catch {
                logoutUser();
                navigate("/login", { replace: true });
            } finally {
                setLoading(false);
            }
        };

        fetchProfile();
    }, [navigate]);

    const handleLogout = () => {
        logoutUser();
        navigate("/login", { replace: true });
    };

    if (loading) {
        return <div className="dashboard">Loading profile...</div>;
    }

    if (!profile) {
        return <div className="dashboard">{error}</div>;
    }

    return (
        <div className="dashboard">
            <header className="dashboard-header">
                <h2>My Dashboard</h2>
                <button onClick={handleLogout} className="logout-button">
                    Logout
                </button>
            </header>

            <div className="welcome-card">
                <h1>Welcome, {profile.name}!</h1>
                <p>You have successfully logged in.</p>

                <div className="profile-details">
                    <p><strong>User ID:</strong> {profile.id}</p>
                    <p><strong>Name:</strong> {profile.name}</p>
                    <p><strong>Email:</strong> {profile.email}</p>
                </div>
            </div>
        </div>
    );
}

export default Dashboard;
