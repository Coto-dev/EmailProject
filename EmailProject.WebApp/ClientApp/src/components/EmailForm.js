import React, { useState } from 'react';
import axios from 'axios';
import { useInterval } from 'react-use';

export const EmailForm = () => {
    const [email, setEmail] = useState('');
    const [code, setCode] = useState('');
    const [showCodeInput, setShowCodeInput] = useState(false);
    const [message, setMessage] = useState('');
    const [timer, setTimer] = useState(0);

    useInterval(() => {
        if (timer > 0) {
            setTimer(timer - 1);
        }
    }, 1000);   
    
    const handleEmailSubmit = async (e) => {
        e.preventDefault();
        if (timer > 0) {
            setMessage(`Please wait ${timer} seconds before trying again.`);
            return;
        }
            await axios.post('/auth/register', { email })
                .then((response) => {
                    console.log(response)
                    if (response.status === 200) {
                        setTimer(3);
                        setMessage('Verification successful');
                        setShowCodeInput(true);
                    }
                }).catch(error => {
                    setTimer(3);
                    setMessage('Invalid verification code');
                });
     
    };

    const handleCodeSubmit = async (e) => {
        e.preventDefault();
        if (timer > 0) {
            setMessage(`Please wait ${timer} seconds before trying again.`);
            return;
        }
            await axios.post('/auth/verify', { email, code })
                .then((response) => {
                    setTimer(3);
                    console.log(response)
                    if (response.status === 200) {
                        setMessage('Verification successful');
                        setShowCodeInput(false);
                    } 
                }).catch(error => {
                    setTimer(3);
                    setMessage('Invalid verification code');
                });
    };

    return (
        <div>
            <form onSubmit={handleEmailSubmit}>
                <div>
                    <label>Email: </label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <button className={"btn-primary"} type="submit">Continue</button>
            </form>
            {showCodeInput && (
                <form onSubmit={handleCodeSubmit}>
                    <div>
                        <label>Verification Code: </label>
                        <input
                            type="text"
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            required
                        />
                    </div>
                    <button className={"btn-primary"} type="submit">Verify</button>
                </form>
            )}
            {message && <p>{message}</p>}
        </div>
    );
};

