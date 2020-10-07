import React, { useState, useEffect } from 'react';
import RingLoader from "react-spinners/RingLoader";
import Axios from 'axios';
import swal from 'sweetalert';

interface QueuedVideo {
    channelId: string;
    channelTitle: string;
    description: string;
    etag: string;
    highThumbnail: string;
    language: string
    publishedAt: Date;
    videoId: string;
    videoTitle: string;
}
export function Queue() {
    const [loading, setLoading] = useState<boolean>(false);
    const [results, setResults] = useState<QueuedVideo[]>();

    useEffect(() => {
        try {
            setLoading(true);
            Axios.get('https://localhost:5001/api/queue/')
                .then((res) => {
                    setResults(res.data);
                });
        } catch (error) {
            swal({ text: 'Some error happened', timer: 5000 });
        } finally {
            setLoading(false);
        }
    }, []);
    const videos = results?.map(r =>
        <>
            <div key={r.videoId}>
                <h3>{r.videoTitle}</h3>
                <img src={r.highThumbnail} />
            </div>
            <br />
        </>
    );
    return <>
        <RingLoader
            size={150}
            color={"#123abc"}
            loading={loading}
        />
        {videos}
    </>
}